using System;
using UniRx;
using UnityEngine;

public interface ICardForwarder
{
    IObservable<Unit> WhenForwarded { get; }
    
    bool CanForward(ICard card, ISlot fromUserDestination);
    IObservable<Unit> Forward(ICard card, ISlot fromUserDestination);
}

public class CardForwarder : ICardForwarder
{
    private readonly Subject<Unit> forwarding = new Subject<Unit>();
    private readonly IShip ship;
    private readonly IAudioManager audioManager;

    private CardForwarder(
        IShip ship, 
        IAudioManager audioManager
        )
    {
        this.ship = ship;
        this.audioManager = audioManager;
    }

    public IObservable<Unit> WhenForwarded => forwarding;
    
    public bool CanForward(ICard card, ISlot fromUserDestination)
    {
        return card.IsResource && 
               !card.IsBoarded && 
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

    private IObservable<Unit> Forward(ICard card)
    {
        if (card.IsResource)
        {
            var forwarding = card.Reveal()
                .DoOnSubscribe(() =>
                {
                    card.IsBoarded = true;
                    card.IsStored = true;
                    
                    if (card.IsMonster)
                        audioManager.Play(AudioEventKey.CardRevealMonster);
                    else
                        audioManager.Play(AudioEventSwitchKey.CardReveal, card.Type);
                });
            
            if (card.IsItem)
                return forwarding.Merge(ship.Storage.Lodge(card));

            if (card.IsTool)
                return forwarding.Merge(ship.Mount.Lodge(card));
        }

        throw new NotImplementedException();
    }
}