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

public enum SlotEntryway
{
    Front, 
    Rear
}

public interface ISlot : ICardBind
{
    bool IsVisible { get; set; }
    bool IsLocked { get; set; }
    SlotType Type { get; }
    SlotEntryway Entryway { get; set; }
    CardType EntryMask { get; }
    Vector3 Position { get; set; }
    Bounds Bounds { get; set; }

    IObservable<ICard> WhenCardPicked { get; }
    IObservable<ICard> WhenLodged { get; }
    IObservable<bool> Highlighting { get; }
    IObservable<bool> Visibility { get; }
    IObservable<bool> Locking { get; }

    ICard Pick();
    bool CanApply(ICard card, ISlot fromSlot);
    void Apply(ICard card);
    bool CanLodge(ICard card, ISlot fromSlot);
    void Lodge(ICard card);
    void Arrange();
    void ToggleHighlight(bool on);
    bool DoesContain(Vector3 worldPoint);
}

public interface ICardBind
{
    void Release(ICard card);
}

public abstract class Slot : ISlot
{
    private readonly IPile pile;
    private readonly Subject<ICard> picking = new Subject<ICard>();
    private readonly Subject<ICard> lodging = new Subject<ICard>();
    private readonly Subject<bool> highlighting = new Subject<bool>();
    private readonly ReactiveProperty<bool> isVisible = new ReactiveProperty<bool>(true);
    private readonly ReactiveProperty<bool> isLocked = new ReactiveProperty<bool>(false);
    private Vector3 position;
    
    protected Slot(SlotType type, IPile pile)
    {
        this.pile = pile;
        Type = type;
    }

    public abstract CardType EntryMask { get; }
    public SlotType Type { get; }
    public SlotEntryway Entryway { get; set; }
    public Bounds Bounds { get; set; }
    
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

    public Vector3 Position
    {
        get => position;
        set => position = pile.Position = value;
    }

    public IObservable<ICard> WhenCardPicked => picking;
    public IObservable<ICard> WhenLodged => lodging;
    public IObservable<bool> Highlighting => highlighting.DistinctUntilChanged();
    public IObservable<bool> Visibility => isVisible;
    public IObservable<bool> Locking => isLocked;

    public ICard Pick()
    {
        var card = pile.Peek();
        
        card?.Pick();
        
        if (card != null)
            picking.OnNext(card);

        return card;
    }
    
    public virtual bool CanLodge(ICard card)
    {        
        return card != null && pile.CanInsert && !pile.DoesContain(card) && (EntryMask & card.Type) != 0;
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
        pile.Insert(card, Entryway == SlotEntryway.Front ? PileInsertionMode.Unshift : PileInsertionMode.Push);
        
        card.Bind(this);
        
        lodging.OnNext(card);
    }

    public void Release(ICard card)
    {
        pile.Remove(card);
    }

    public void Arrange()
    {
        pile.Arrange();
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