using System;
using DG.Tweening;
using UniRx;
using UnityEngine;

public interface IDefermentController : IDisposable
{
    IObservable<ArtificeType> WhenMatchedDevice { get; }
    IObservable<ArtificeType> WhenDeviceActed { get; }
    IObservable<Unit> WhenResupplied { get; }
    
    bool CanDefer(ISlot fromSource, ISlot atDestination);
    
    IObservable<Unit> Defer(ISlot fromSource, ISlot atDestination);
}

public class DefermentController : IDefermentController
{
    private const float DefermentStepDuration = 0.4f;
    
    private readonly Subject<ArtificeType> deviceMatching = new Subject<ArtificeType>();
    private readonly Subject<ArtificeType> deviceActing = new Subject<ArtificeType>();
    private readonly Subject<Unit> resupplying = new Subject<Unit>();
    
    private readonly IDealingController dealer;
    private readonly IBoardModel boardModel;
    private readonly IBoard board;
    private readonly LodgingSettings plummetSettings;

    private DefermentController(
        IDealingController dealer,
        IBoardModel boardModel,
        IBoard board
    )
    {
        this.dealer = dealer;
        this.boardModel = boardModel;
        this.board = board;
        
        plummetSettings = new LodgingSettings(
            SlotLodgingMode.Default,
            false,
            Ease.InCubic,
            DefermentStepDuration);
    }

    public IObservable<ArtificeType> WhenDeviceActed => deviceActing;
    public IObservable<ArtificeType> WhenMatchedDevice => deviceMatching;
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
                    board.Sea.ZDepth * 0.5f);

                var deferment = deferredCard.Fling(midairPosition, Ease.OutCubic, DefermentStepDuration)
                    .Do(_ => deferredCard.Sort(boardModel.CardCountPerSupplySlot))
                    .Merge(atDestination.Peek().Destroy())
                    .AsSingleUnitObservable();

                var lodging = fromSource.Lodge(deferredCard, plummetSettings)
                    .DoOnCompleted(() =>
                    {
                        deferredCard.Bounce(Vector3.down * 0.5f);
                        deviceActing.OnNext(ArtificeType.Catapult);
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
                    .ContinueWith(board.Sea.Arrange()
                        .Merge(lodging))
                    .Subscribe(observer);
            })
            .DoOnSubscribe(() =>
            {
                deviceMatching.OnNext(ArtificeType.Catapult); // TODO Generalize
                
                board.Ship.Lock();
                board.Sea.Lock();
            })
            .DoOnCompleted(() =>
            {
                board.Ship.Unlock();
                board.Sea.Unlock();
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
               byDestination is IArtificeCard device && 
               (device.ArtificeType & ArtificeType.Catapult) != 0;
    }
}