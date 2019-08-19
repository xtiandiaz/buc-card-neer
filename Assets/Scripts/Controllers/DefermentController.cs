using System;
using UniRx;

public interface IDefermentController : IDisposable
{
    IObservable<DeviceType> WhenMatchedDevice { get; }
    IObservable<DeviceType> WhenDeviceActed { get; }
    
    bool CanDefer(ISlot fromSource, ISlot atDestination);
    
    IObservable<Unit> Defer(ISlot fromSource, ISlot atDestination);
}

public class DefermentController : IDefermentController
{
    private readonly Subject<DeviceType> deviceMatching = new Subject<DeviceType>();
    private readonly Subject<DeviceType> deviceActing = new Subject<DeviceType>();
    
    private readonly IDealingController dealer;
    private readonly IBoardModel boardModel;
    private readonly IShip ship;
    private readonly ISea sea;

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
    }

    public IObservable<DeviceType> WhenDeviceActed => deviceActing;
    public IObservable<DeviceType> WhenMatchedDevice => deviceMatching;
    
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
                var matchingAndLodging = fromSource.Lodge(
                        deferredCard,
                        SlotLodgingMode.Systematic,
                        !fromSource.IsEmpty)
                    .DoOnCompleted(() => deviceActing.OnNext(DeviceType.Catapult))
                    .Merge(atDestination.Peek().Destroy())
                    .AsSingleUnitObservable();

                if (fromSource.IsEmpty)
                {
                    return dealer.Deal(boardModel.CardCountPerSupplySlot - 1, fromSource)
                        .ContinueWith(matchingAndLodging)
                        .Subscribe(observer);
                }

                return matchingAndLodging
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