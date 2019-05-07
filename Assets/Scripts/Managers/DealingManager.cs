using System;
using UniRx;

public interface IDealingManager
{
    IObservable<Unit> Deal(IDeck fromDeck, int count, TimeSpan withInterval, params ISlot[] inSlots);
}

public class DealingManager : IDealingManager
{
    public IObservable<Unit> Deal(IDeck fromDeck, int count, TimeSpan withInterval, params ISlot[] inSlots)
    {
        return Observable.Timer(TimeSpan.Zero, withInterval)
            .Take(count * inSlots.Length)
            .Select(i => new {Card = fromDeck.Supply(), Slot = inSlots[i % inSlots.Length]})
            .TakeWhile(cardForSlot => cardForSlot.Card != null)
            .Do(cardForSlot => cardForSlot.Slot.Lodge(cardForSlot.Card))
            .AsUnitObservable();
    }
}