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
    Bounds Bounds { get; set; }
    ICardArrangement Arrangement { get; set; }
    ICard[] Cards { get; }

    IObservable<ICard> Lodged { get; }
    IObservable<bool> BecameHighlighted { get; }
    IObservable<bool> BecameVisible { get; }

    bool CanLodge(ICard card);
    void Lodge(ICard card);
    void Release(ICard card);
    void ToggleHighlight(bool on);
    bool DoesContain(Vector3 worldPoint);
}

public abstract class Slot : ISlot
{
    private readonly Stack<ICard> cards = new Stack<ICard>();
    private readonly Subject<ICard> lodged = new Subject<ICard>();
    private readonly Subject<bool> highlighted = new Subject<bool>();
    private readonly ReactiveProperty<bool> isVisible = new ReactiveProperty<bool>(true);
    
    private Vector3 position;
    
    protected Slot(SlotType type, uint capacity)
    {
        Capacity = capacity;
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
    public Bounds Bounds { get; set; }
    public ICardArrangement Arrangement { get; set; }
    public ICard[] Cards => cards.ToArray();

    public IObservable<ICard> Lodged => lodged;
    public IObservable<bool> BecameHighlighted => highlighted.DistinctUntilChanged();
    public IObservable<bool> BecameVisible => isVisible;

    public bool CanLodge(ICard card)
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

        cards.Push(card);
        
        lodged.OnNext(card);
    }

    public void Release(ICard card)
    {
        cards.Pop();
    }

    public void ToggleHighlight(bool on)
    {
        highlighted.OnNext(on);
    }

    public bool DoesContain(Vector3 worldPoint)
    {
        return Bounds.Contains(worldPoint);
    }
}