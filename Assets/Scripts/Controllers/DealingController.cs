using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Zenject;

public interface IDealingController : IInitializable, IDisposable
{
    IObservable<int> ActiveCardCount { get; }
    IObservable<int> UndealtCardCount { get; }
    
    IObservable<ICard> WhenDealt { get; }
    
    bool CanDeal(ISlot intoSlot);
    bool IsThereDeadlock();
    
    IObservable<Unit> Deal(int count, ISlot intoSlot, double atIntervalsOfDuration = 0.1);
    IObservable<Unit> Deal(IEnumerable<ArtificeType> devices, ISlot intoSlot, double atIntervalsOfDuration = 0.1);
}

public class DealingController : IDealingController
{
    private readonly Subject<ICard> dealing = new Subject<ICard>();
    private readonly ReactiveProperty<int> activeCardCount = new ReactiveProperty<int>();
    private readonly Dictionary<CardType, int> activeCardTypes = new Dictionary<CardType, int>();
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private readonly IDeck deck;
    private readonly ICardFactory cardFactory;

    private DealingController(
        IDeck deck,
        ICardFactory cardFactory
        )
    {
        this.deck = deck;
        this.cardFactory = cardFactory;

        UndealtCardCount = deck.CardCount;
    }
    
    public IObservable<int> ActiveCardCount => activeCardCount;
    public IObservable<int> UndealtCardCount { get; }
    
    public IObservable<ICard> WhenDealt => dealing;

    public void Initialize()
    {
        disposables.Add(dealing
            .Do(card => OnCardDealt(card.AbstractType))
            .SelectMany(card => (card.IsLocked 
                ? card.WhenUnlocked.Do(_ => OnCardUnlocked(card)).ContinueWith(card.WhenDestroyed)
                : card.WhenDestroyed).Select(_ => card))
            .Subscribe(card => OnCardDestroyed(card.Type)));
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

    public IObservable<Unit> Deal(int count, ISlot intoSlot, double atIntervalsOfDuration = 0.1)
    {
        return Deal(deck.Provide(count), intoSlot, atIntervalsOfDuration);
    }
    
    public IObservable<Unit> Deal(IEnumerable<ArtificeType> devices, ISlot intoSlot, double atIntervalsOfDuration = 0.1)
    {
        return Deal(devices.Select(cardFactory.Create), intoSlot, atIntervalsOfDuration);
    }

    public void Dispose()
    {
        dealing.Dispose();
        
        disposables.Dispose();
    }

    private IObservable<Unit> Deal(IEnumerable<ICard> cards, ISlot intoSlot, double atIntervalsOfDuration)
    {
        return cards
            .Select((card, index) => 
            { 
                dealing.OnNext(card);

                return intoSlot.Lodge(card)
                    .DelaySubscription(TimeSpan.FromSeconds(atIntervalsOfDuration * index));
            })
            .Merge()
            .AsSingleUnitObservable();
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

    private void OnCardUnlocked(ICard card)
    {
        OnCardDestroyed(card.AbstractType);
        
        OnCardDealt(card.Type);
    }

    private void OnCardDestroyed(CardType withType)
    {
        activeCardTypes[withType]--;

        activeCardCount.Value--;
    }
}