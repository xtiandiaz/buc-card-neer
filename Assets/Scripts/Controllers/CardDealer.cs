using System;
using UniRx;

public interface ICardDealer
{
    bool CanDeal(ISlot intoSlot);
    
    IObservable<Unit> DealOne(ISlot intoSlot);
    IObservable<Unit> DealOne(CardType ofType, ISlot intoSlot);
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
        return Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(0.1))
            .Take(count)
            .SelectMany(_ => DealOne(intoSlot))
            .AsSingleUnitObservable();
    }
    
    public IObservable<Unit> DealOne(ISlot intoSlot)
    {
        return deck.IsExhausted ? Observable.Empty<Unit>() : intoSlot.Lodge(deck.Provide());
    }

    public IObservable<Unit> DealOne(CardType ofType, ISlot intoSlot)
    {
        return Observable.Create<Unit>(observer =>
        {
            if (deck.DoesContain(ofType))
            {
                return intoSlot.Lodge(deck.Provide(ofType))
                    .Subscribe(observer);
            }
            
            observer.OnCompleted();

            return Disposable.Empty;
        });
    }
}