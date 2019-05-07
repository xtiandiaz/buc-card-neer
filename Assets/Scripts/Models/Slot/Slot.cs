using System;
using UniRx;
using UnityEngine;

[Flags]
public enum SlotType
{
    Event     = 1 << 0,
    Boarding  = 1 << 1,
    Storage   = 1 << 2,
    Player    = 1 << 3
}

public interface ISlot : ICardBind
{
    bool IsVisible { get; set; }
    bool IsLocked { get; set; }
    SlotType Type { get; }
    CardType EntryMask { get; }
    CardFace SlottingFace { get; set; }
    Vector3 Position { get; set; }
    Bounds Bounds { get; set; }
    ICardArrangement Arrangement { get; set; }

    IObservable<(ICard, ISlot)> WhenCardPicked { get; }
    IObservable<(ICard, ISlot, Vector3)> WhenCardDropped { get; }
    IObservable<ICard> Taking { get; }
    IObservable<bool> Highlighting { get; }
    IObservable<bool> Visibility { get; }
    IObservable<bool> Locking { get; }

    ICard Pick();
    //void Drop(ICard card);
    bool CanApply(ICard card, ISlot fromSlot);
    void Apply(ICard card);
    bool CanLodge(ICard card, ISlot fromSlot);
    void Lodge(ICard card);
    void ToggleHighlight(bool on);
    bool DoesContain(Vector3 worldPoint);
}

public interface ICardBind
{
    void Release(ICard card);
}

public abstract class Slot : ISlot, ICardBind
{
    private static readonly Subject<(ICard, ISlot)> Picking = new Subject<(ICard, ISlot)>();
    private static readonly Subject<(ICard, ISlot, Vector3)> Dropping = new Subject<(ICard, ISlot, Vector3)>();
    
    private readonly IPile pile;
    private readonly Subject<bool> highlighting = new Subject<bool>();
    private readonly Subject<ICard> taking = new Subject<ICard>();
    private readonly ReactiveProperty<bool> isVisible = new ReactiveProperty<bool>(true);
    private readonly ReactiveProperty<bool> isLocked = new ReactiveProperty<bool>(false);
    private Vector3 position;
    
    protected Slot(SlotType type, IPile pile)
    {
        this.pile = pile;
        Type = type;
    }

    public abstract CardType EntryMask { get; }
    public uint Capacity { get; }
    public bool IsVisible
    {
        get => isVisible.Value;
        set => isVisible.Value = value;
    }
    
    public bool IsLocked
    {
        get => isLocked.Value;
        set => isLocked.Value = value;
    }

    public SlotType Type { get; }
    public CardFace SlottingFace { get; set; }

    public Vector3 Position
    {
        get => position;
        set => position = pile.Position = value;
    }
    
    public Bounds Bounds { get; set; }
    public ICardArrangement Arrangement { get; set; }

    public IObservable<(ICard, ISlot)> WhenCardPicked => Picking;
    public IObservable<(ICard, ISlot, Vector3)> WhenCardDropped => Dropping;
    public IObservable<ICard> Taking => taking;
    public IObservable<bool> Highlighting => highlighting.DistinctUntilChanged();
    public IObservable<bool> Visibility => isVisible;
    public IObservable<bool> Locking => isLocked;

    public ICard Pick()
    {
        var card = pile.Peek();
        
        card?.Pick();
        
        if (card != null)
            Picking.OnNext((card, this));

        return card;
    }
    
    public virtual bool CanLodge(ICard card)
    {        
        return card != null && pile.CanAdd && !pile.DoesContain(card) && (EntryMask & card.Type) != 0;
    }

    public virtual bool CanLodge(ICard card, ISlot fromSlot)
    {
        return CanLodge(card);
    }

    public bool CanApply(ICard card, ISlot fromSlot)
    {
        return pile.Peek()?.DoesMatch(card) == true;
    }

    public void Apply(ICard card)
    {
        pile.Peek()?.Match(card);
    }

    public void Lodge(ICard card)
    {
        pile.Add(card);
        
        card.Bind(this);
        
        taking.OnNext(card);
    }

    public void Release(ICard card)
    {
        pile.Remove(card);
    }

    public void ToggleHighlight(bool on)
    {
        highlighting.OnNext(on);
    }
    
    public bool DoesContain(Vector3 worldPoint)
    {
        return Bounds.Contains(worldPoint);
    }
}