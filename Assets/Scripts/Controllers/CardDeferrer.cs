using System;
using UniRx;

public interface ICardDeferrer
{
    bool CanDefer(ISlot fromSource, ISlot atDestination);
    IObservable<Unit> Defer(ISlot fromSource, ISlot atDestination);
}

public class CardDeferrer : ICardDeferrer
{
    public bool CanDefer(ISlot fromSource, ISlot atDestination)
    {
        return (atDestination.Type & SlotType.Boarding) != 0 &&
               CanDefer(fromSource.Peek(), atDestination.Peek());
    }

    public IObservable<Unit> Defer(ISlot fromSource, ISlot atDestination)
    {
        return Observable.ReturnUnit();
    }

    private bool CanDefer(ICard source, ICard byDestination)
    {
        if (source == null || byDestination == null)
            return false;

        return !source.IsBoarded && 
               (byDestination.Type & CardType.Trap) != 0;
    }
}