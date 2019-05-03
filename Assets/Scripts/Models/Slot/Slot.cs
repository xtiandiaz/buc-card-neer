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
    Resource  = 1 << 3,
    Player    = 1 << 4,
    
    All = Event | Boarding | Defense | Resource | Player
}

public interface ISlot
{
    uint Capacity { get; }
    bool HasRoom { get; }
    bool IsVisible { get; set; }
    SlotType Type { get; }
    CardType EntryMask { get; }
    Vector3 Position { get; set; }
    Bounds Bounds { get; set; }
    ICardArrangement Arrangement { get; set; }

    IObservable<ICard> Lodged { get; }
    IObservable<bool> BecameHighlighted { get; }
    IObservable<bool> BecameVisible { get; }

    bool CanLodge(ICard card);
    void Lodge(ICard card);
    void Release(ICard card);
    void ArrangeCards();
    void ToggleHighlight(bool on);
    bool DoesContain(ICard card);
    bool DoesContain(Vector3 worldPoint);
}

public abstract class Slot : ISlot
{
    private readonly List<ICard> cards = new List<ICard>();
    private readonly Subject<ICard> lodged = new Subject<ICard>();
    private readonly Subject<bool> highlighted = new Subject<bool>();
    private readonly ReactiveProperty<bool> isVisible = new ReactiveProperty<bool>(true);
    
    protected Slot(SlotType type, uint capacity)
    {
        Capacity = capacity > 0 ? capacity : uint.MaxValue;
        Type = type;
    }

    public abstract CardType EntryMask { get; }
    public uint Capacity { get; }
    public bool HasRoom => cards.Count < Capacity;
    public bool IsVisible
    {
        get => isVisible.Value;
        set => isVisible.Value = value;
    }

    public SlotType Type { get; }
    public Vector3 Position { get; set; }
    public Bounds Bounds { get; set; }
    public ICardArrangement Arrangement { get; set; }

    public IObservable<ICard> Lodged => lodged;
    public IObservable<bool> BecameHighlighted => highlighted.DistinctUntilChanged();
    public IObservable<bool> BecameVisible => isVisible;

    public virtual bool CanLodge(ICard card)
    {
        return HasRoom && !cards.Contains(card) && (EntryMask & card.Type) != 0;
    }

    public void Lodge(ICard card)
    {
        if (!CanLodge(card))
        {
            Debug.LogWarning($"[Slot] Card '{card.Name}' couldn't be lodged.");
            return;
        }
        
        cards.Add(card);
        lodged.OnNext(card);
        
        ArrangeCards();
    }

    public void Release(ICard card)
    {
        if (cards.Remove(card))
            ArrangeCards();
    }
    
    public void ArrangeCards()
    {
        var cardCountM1 = cards.Count - 1;
        var capacity = (int) Capacity;

        for (var i = cardCountM1; i >= 0; i--)
        {
            var index = cardCountM1 - i;
            
            cards[i].Arrange(Arrangement.Transform(Position, index, capacity), index);
            Arrangement.Decorate(cards[i], index, capacity);
        }
    }

    public void ToggleHighlight(bool on)
    {
        highlighted.OnNext(on);
    }

    public bool DoesContain(ICard card)
    {
        return cards.Contains(card);
    }
    
    public bool DoesContain(Vector3 worldPoint)
    {
        return Bounds.Contains(worldPoint);
    }
}