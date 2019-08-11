using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Zenject;

public interface ICardDealer : IInitializable, IDisposable
{
    IObservable<int> ActiveCardCount { get; }
    
    bool CanDeal(ISlot intoSlot);
    bool IsThereDeadlock();
    
    IObservable<Unit> Deal(int count, ISlot intoSlot);
}

public class CardDealer : ICardDealer
{
    private readonly Subject<ICard> dealing = new Subject<ICard>();
    private readonly ReactiveProperty<int> activeCardCount = new ReactiveProperty<int>();
    private readonly Dictionary<CardType, int> activeCardTypes = new Dictionary<CardType, int>();
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private readonly IDeck deck;

    private CardDealer(IDeck deck)
    {
        this.deck = deck;
    }
    
    public IObservable<int> ActiveCardCount => activeCardCount;

    public void Initialize()
    {
        disposables.Add(dealing
            .Do(card => OnCardDealt(card.AbstractType))
            .SelectMany(card => card.IsMonster 
                ? card.WhenUnlocked.Select(_ => CardType.Monster)
                : card.WhenDestroyed.Select(_ => card.Type))
            .Subscribe(OnCardDestroyedOrUnlocked));
    }
    
    public bool CanDeal(ISlot intoSlot)
    {
        return !deck.IsExhausted && intoSlot.HasRoom;
    }

    public bool IsThereDeadlock()
    {
        if (GetActiveCardCount(CardType.Pirate | CardType.Monster) > 0)
            return false;
        
        if (GetActiveCardCount(CardType.Resource) > 0 && GetActiveCardCount(CardType.Merchant) > 0)
            return false;
        
        return true;
    }

    public IObservable<Unit> Deal(int count, ISlot intoSlot)
    {
        return deck.Provide(count)
            .Select((card, index) => 
            { 
                dealing.OnNext(card);

                return intoSlot.Lodge(card)
                    .DelaySubscription(TimeSpan.FromSeconds(0.1 * index));
            })
            .Merge()
            .AsSingleUnitObservable();
    }

    public void Dispose()
    {
        dealing.Dispose();
        
        disposables.Dispose();
    }
    
    private int GetActiveCardCount(CardType ofType)
    {
        return activeCardTypes.Where(pair => (pair.Key & ofType) != 0).Sum(pair => pair.Value);
    }

    private void OnCardDealt(CardType ofType)
    {
        if (!activeCardTypes.ContainsKey(ofType))
            activeCardTypes.Add(ofType, 0);

        activeCardTypes[ofType]++;
        
        activeCardCount.Value++;
    }

    private void OnCardDestroyedOrUnlocked(CardType ofType)
    {
        activeCardTypes[ofType]--;

        activeCardCount.Value--;
    }
}