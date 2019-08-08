using System;
using UniRx;

public interface ICardDismisser : IDisposable
{
    IObservable<Unit> WhenCardDismissed { get; }
    
    bool CanDismiss(ISlot fromSource);
    
    IObservable<Unit> Dismiss(ISlot fromSource);
}

public class CardDismisser : ICardDismisser
{
    private readonly Subject<Unit> dismissing = new Subject<Unit>();

    public IObservable<Unit> WhenCardDismissed => dismissing;

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
                return fromSource.Pop()
                    .Destroy()
                    .DoOnSubscribe(() => dismissing.OnNext(Unit.Default))
                    .Subscribe(observer);
            
            observer.OnCompleted();
            
            return Disposable.Empty;
        });
    }

    public void Dispose()
    {
        dismissing.Dispose();
    }

    private bool CanDismiss(ICard card)
    {
        return card != null && (card.Type & CardType.Merchant) != 0;
    }
}