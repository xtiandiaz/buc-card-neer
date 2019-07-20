using System;
using UniRx;
using UnityEngine;

public interface ICardDealer
{
    bool CanDeal(ISlot intoSlot);
    
    IObservable<Unit> Deal(int count, ISlot intoSlot);
}

public class CardDealer : ICardDealer
{
    private readonly IDeck deck;

    private CardDealer(IDeck deck)
    {
        this.deck = deck;
    }

    public bool CanDeal(ISlot intoSlot)
    {
        return !deck.IsExhausted && intoSlot.HasRoom;
    }

    public IObservable<Unit> Deal(int count, ISlot intoSlot)
    {
        return deck.Provide(count)
            .ToObservable()
            .SelectMany((card, index) => intoSlot.Lodge(card)
                .DelaySubscription(TimeSpan.FromSeconds(0.1 * index)))
            .DoOnError(Debug.LogException)
            .AsSingleUnitObservable();
    }
}