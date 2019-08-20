using System;
using DG.Tweening;
using UniRx;
using UnityEngine;

public interface IDefermentController : IDisposable
{
    IObservable<DeviceType> WhenMatchedDevice { get; }
    IObservable<DeviceType> WhenDeviceActed { get; }
    IObservable<Unit> WhenResupplied { get; }
    
    bool CanDefer(ISlot fromSource, ISlot atDestination);
    
    IObservable<Unit> Defer(ISlot fromSource, ISlot atDestination);
}

public class DefermentController : IDefermentController
{
    private const float DefermentStepDuration = 0.4f;
    
    private readonly Subject<DeviceType> deviceMatching = new Subject<DeviceType>();
    private readonly Subject<DeviceType> deviceActing = new Subject<DeviceType>();
    private readonly Subject<Unit> resupplying = new Subject<Unit>();
    
    private readonly IDealingController dealer;
    private readonly IBoardModel boardModel;
    private readonly IShip ship;
    private readonly ISea sea;
    private readonly LodgingSettings plummetSettings;

    private DefermentController(
        IDealingController dealer,
        IBoardModel boardModel,
        IShip ship,
        ISea sea
    )
    {
        this.dealer = dealer;
        this.boardModel = boardModel;
        this.ship = ship;
        this.sea = sea;
        
        plummetSettings = new LodgingSettings(
            SlotLodgingMode.Default,
            false,
            Ease.InCubic,
            DefermentStepDuration);
    }

    public IObservable<DeviceType> WhenDeviceActed => deviceActing;
    public IObservable<DeviceType> WhenMatchedDevice => deviceMatching;
    public IObservable<Unit> WhenResupplied => resupplying;
    
    public bool CanDefer(ISlot fromSource, ISlot atDestination)
    {
        return (atDestination.Type & SlotType.Boarding) != 0 &&
               CanDefer(fromSource.Peek(), atDestination.Peek());
    }

    public IObservable<Unit> Defer(ISlot fromSource, ISlot atDestination)
    {
        return Observable.Create<Unit>(observer =>
            {
                var deferredCard = fromSource.Pop();
                var midairPosition = new Vector3(
                    fromSource.Position.x,
                    fromSource.Position.y + GameStatics.CardHeight * 2f,
                    sea.ZDepth * 0.5f);

                var deferment = deferredCard.Fling(midairPosition, Ease.OutCubic, DefermentStepDuration)
                    .Do(_ => deferredCard.Sort(boardModel.CardCountPerSupplySlot))
                    .Merge(atDestination.Peek().Destroy())
                    .AsSingleUnitObservable();

                var lodging = fromSource.Lodge(deferredCard, plummetSettings)
                    .DoOnCompleted(() =>
                    {
                        deferredCard.Bounce(Vector3.down * 0.5f);
                        deviceActing.OnNext(DeviceType.Catapult);
                    });

                if (fromSource.IsEmpty)
                {
                    return deferment
                        .Do(resupplying)
                        .ContinueWith(dealer.Deal(boardModel.CardCountPerSupplySlot - 1, fromSource))
                        .ContinueWith(lodging)
                        .Subscribe(observer);
                }

                return deferment
                    .ContinueWith(sea.Arrange()
                        .Merge(lodging))
                    .Subscribe(observer);
            })
            .DoOnSubscribe(() =>
            {
                deviceMatching.OnNext(DeviceType.Catapult); // TODO Generalize
                
                ship.Lock();
                sea.Lock();
            })
            .DoOnCompleted(() =>
            {
                ship.Unlock();
                sea.Unlock();
            });
    }

    public void Dispose()
    {
        deviceActing.Dispose();
        deviceMatching.Dispose();
        resupplying.Dispose();
    }

    private bool CanDefer(ICard source, ICard byDestination)
    {
        if (source == null || byDestination == null)
            return false;

        return !source.IsBoarded && 
               byDestination is IDeviceCard device && 
               (device.DeviceType & DeviceType.Catapult) != 0;
    }
}