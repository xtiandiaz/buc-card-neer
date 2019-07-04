using System;
using UniRx;

public interface ICardDismisser
{
    bool CanDismiss(ISlot fromSource);
    IObservable<Unit> Dismiss(ISlot fromSource);
}

public class CardDismisser : ICardDismisser
{
    public bool CanDismiss(ISlot fromSource)
    {
        return (fromSource.Type & SlotType.Boarding) != 0 &&
               CanDismiss(fromSource.Peek());
    }

    public IObservable<Unit> Dismiss(ISlot fromSource)
    {
        return Observable.Create<Unit>(observer =>
        {
            if (CanDismiss(fromSource))
                return fromSource.Pop().Destroy().Subscribe(observer);
            
            observer.OnCompleted();
            
            return Disposable.Empty;
        });
    }

    private bool CanDismiss(ICard card)
    {
        return card != null && (card.Type & CardType.Merchant) != 0;
    }
}