using System;
using UniRx;
using UnityEngine;

public interface IForwardingController : IDisposable
{
    IObservable<Unit> WhenForwarded { get; }
    IObservable<CardType> WhenCardStashed { get; }
    IObservable<CardType> WhenCardRevealed { get; }
    
    bool CanForward(ICard card, ISlot fromUserDestination);
    IObservable<Unit> Forward(ICard card, ISlot fromUserDestination);
}

public class ForwardingController : IForwardingController
{
    private readonly Subject<Unit> forwarding = new Subject<Unit>();
    private readonly Subject<CardType> revealing = new Subject<CardType>();
    private readonly Subject<CardType> stashing = new Subject<CardType>();
    private readonly IBoard board;
    private readonly IGameStatus gameStatus;

    private ForwardingController(
        IBoard board,
        IGameStatus gameStatus
        )
    {
        this.board = board;
        this.gameStatus = gameStatus;
    }

    public IObservable<Unit> WhenForwarded => forwarding;
    public IObservable<CardType> WhenCardStashed => stashing;
    public IObservable<CardType> WhenCardRevealed => revealing;
    
    public bool CanForward(ICard card, ISlot fromUserDestination)
    {
        if (!card.IsResource || card.IsLocked || card.IsArtifice)
            return false;
        
        if (card.IsItem && !gameStatus.PlayerDidStashItem)
            return false;
        
        if (card.IsTool && !gameStatus.PlayerDidStashTool)
            return false;
        
        return !card.IsBoarded &&
               !card.IsStashed &&
               (fromUserDestination.Type & SlotType.Boarding) != 0;
    }

    public IObservable<Unit> Forward(ICard card, ISlot fromUserDestination)
    {
        return Observable.Create<Unit>(observer =>
        {
            if (!CanForward(card, fromUserDestination))
            {
                observer.OnError(new Exception("Can't forward Card"));

                return Disposable.Empty;
            }

            return Forward(card)
                .LastOrDefault()
                .Do(forwarding.OnNext)
                .Subscribe(observer);
            
        }).DoOnError(Debug.LogException);
    }
    
    public void Dispose()
    {
        forwarding.Dispose();
        revealing.Dispose();
        stashing.Dispose();
    }

    private IObservable<Unit> Forward(ICard card)
    {
        
        return Observable.Create<Unit>(observer =>
            {
                if (card.IsResource)
                {
                    return board.Ship.ExpressHandle(card)
                        .Subscribe(observer);
                }

                observer.OnError(new Exception($"There was no match to forward resource '{card.Type}'."));
                
                return Disposable.Empty;
            })
            .AsSingleUnitObservable();
    }
}