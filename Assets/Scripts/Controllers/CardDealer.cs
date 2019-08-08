using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public interface ICardDealer : IDisposable
{
    IObservable<int> ActiveCardCount { get; }
    
    bool CanDeal(ISlot intoSlot);
    bool IsThereDeadlock();
    
    IObservable<Unit> Deal(int count, ISlot intoSlot);
}

public class CardDealer : ICardDealer
{
    private readonly IDeck deck;
    private readonly ReactiveProperty<int> activeCardCount = new ReactiveProperty<int>();
    private readonly Dictionary<CardType, int> activeCardTypes = new Dictionary<CardType, int>();
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    public IObservable<int> ActiveCardCount => activeCardCount;

    private CardDealer(IDeck deck)
    {
        this.deck = deck;
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

    public int GetActiveCardCount(CardType ofType)
    {
        return activeCardTypes.Where(pair => (pair.Key & ofType) != 0).Sum(pair => pair.Value);
    }

    public IObservable<Unit> Deal(int count, ISlot intoSlot)
    {
        return deck.Provide(count)
            .Select((card, index) => 
            {
                OnCardDealt(card.IsMonster ? CardType.Monster : card.Type);
                
                disposables.Add((card.IsMonster 
                        ? card.WhenUnlocked.Select(_ => CardType.Monster)
                        : card.WhenDestroyed.Select(_ => card.Type))
                    .Subscribe(OnCardDestroyedOrUnlocked));

                return intoSlot.Lodge(card)
                    .DelaySubscription(TimeSpan.FromSeconds(0.1 * index));
            })
            .Merge()
            .DoOnError(Debug.LogException)
            .AsSingleUnitObservable();
    }

    public void Dispose()
    {
        disposables.Dispose();
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