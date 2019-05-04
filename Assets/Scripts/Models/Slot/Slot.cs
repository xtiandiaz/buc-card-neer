using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

[Flags]
public enum SlotType
{
    Event     = 1 << 0,
    Boarding  = 1 << 1,
    Defense   = 1 << 2,
    Storage   = 1 << 3,
    Player    = 1 << 4
}

public interface ISlot
{
    uint Capacity { get; }
    int CardCount { get; }
    bool HasRoom { get; }
    bool IsVisible { get; set; }
    SlotType Type { get; }
    CardType EntryMask { get; }
    Vector3 Position { get; set; }
    Bounds Bounds { get; set; }
    ICard TopCard { get; }
    ICardArrangementSettings ArrangementSettings { get; set; }

    IObservable<ICard> Taking { get; }
    IObservable<bool> Highlighting { get; }
    IObservable<bool> Visibility { get; }
    IObservable<int> LodgingCounting { get; }
    IObservable<Unit> Emptying { get; }

    bool CanTake(ICard card);
    bool CanTake(ICard card, ISlot fromSlot);
    void Take(ICard card);
    void Release(ICard card);
    void ToggleHighlight(bool on);
    bool DoesContain(ICard card);
    bool DoesContain(Vector3 worldPoint);
    void ArrangeCards();
}

public abstract class Slot : ISlot
{
    private readonly List<ICard> cards = new List<ICard>();
    private readonly Subject<bool> highlighting = new Subject<bool>();
    private readonly Subject<ICard> taking = new Subject<ICard>();
    private readonly ReactiveProperty<bool> visibility = new ReactiveProperty<bool>(true);
    
    protected Slot(SlotType type, uint capacity)
    {
        Capacity = capacity > 0 ? capacity : uint.MaxValue;
        Type = type;
    }

    public abstract CardType EntryMask { get; }
    public uint Capacity { get; }
    public int CardCount => cards.Count;
    public bool HasRoom => cards.Count < Capacity;
    public bool IsVisible
    {
        get => visibility.Value;
        set => visibility.Value = value;
    }

    public SlotType Type { get; }
    public Vector3 Position { get; set; }
    public Bounds Bounds { get; set; }
    public ICard TopCard => cards.LastOrDefault();
    public ICardArrangementSettings ArrangementSettings { get; set; }

    public IObservable<ICard> Taking => taking;
    public IObservable<bool> Highlighting => highlighting.DistinctUntilChanged();
    public IObservable<bool> Visibility => visibility;
    public IObservable<int> LodgingCounting => null;
    public IObservable<Unit> Emptying => LodgingCounting.Where(count => count == 0).AsUnitObservable();

    public virtual bool CanTake(ICard card)
    {        
        return card != null && HasRoom && !cards.Contains(card) && (EntryMask & card.Type) != 0;
    }

    public virtual bool CanTake(ICard card, ISlot fromSlot)
    {
        return CanTake(card);
    }

    public void Take(ICard card)
    {
        if (!CanTake(card))
        {
            Debug.LogError($"[Slot] Can't take {card.Name} by in SlotName");
            return;
        }

        cards.Add(card);
        taking.OnNext(card);

        ArrangeCards();
    }

    public void Release(ICard card)
    {
        if (cards.Remove(card))
            ArrangeCards();
    }

    public void ToggleHighlight(bool on)
    {
        highlighting.OnNext(on);
    }

    public bool DoesContain(ICard card)
    {
        return cards.Contains(card);
    }
    
    public bool DoesContain(Vector3 worldPoint)
    {
        return Bounds.Contains(worldPoint);
    }

    public void ArrangeCards()
    {
        var countM1 = cards.Count - 1;
        
        for (var i = countM1; i >= 0; i--)
            cards[i].Arrange(Position, countM1 - i, (int) Capacity, ArrangementSettings);
    }
}