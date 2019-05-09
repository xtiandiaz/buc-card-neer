using System;
using UniRx;
using UnityEngine;

public enum SlotType
{
    Event,
    Boarding,
    Storage,
    Player
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
    Vector3 Position { set; }
    Bounds Bounds { set; }

    IObservable<ICard> WhenPicked { get; }
    IObservable<ICard> WhenLodged { get; }
    IObservable<Unit> WhenReleased { get; }
    IObservable<bool> Highlighting { get; }
    IObservable<bool> Visibility { get; }
    IObservable<bool> Locking { get; }

    ICard Pick();
    bool CanMatch(ICard card);
    void Match(ICard card);
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
    private readonly Subject<Unit> releasing = new Subject<Unit>();
    private readonly Subject<bool> highlighting = new Subject<bool>();
    private readonly ReactiveProperty<bool> isVisible = new ReactiveProperty<bool>(true);
    private readonly ReactiveProperty<bool> isLocked = new ReactiveProperty<bool>(false);
    private Bounds bounds;
    
    protected Slot(SlotType type, IPile pile)
    {
        this.pile = pile;
        Type = type;
    }

    public SlotType Type { get; }
    public SlotEntryway Entryway { get; set; }
    
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
        set => pile.Position = value;
    }
    
    public Bounds Bounds
    {
        set => bounds = value;
    }

    public IObservable<ICard> WhenPicked => picking;
    public IObservable<ICard> WhenLodged => lodging;
    public IObservable<Unit> WhenReleased => releasing;
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

    public bool CanLodge(ICard card, ISlot fromSlot)
    {
        return card != null && pile.CanInsert && !pile.DoesContain(card) && CanLodge(fromSlot) && CanLodge(card);
    }

    public bool CanMatch(ICard card)
    {
        return pile.Peek()?.CanMatch(card) == true;
    }

    public void Match(ICard card)
    {
        pile.Peek()?.Match(card);
    }

    public void Lodge(ICard card)
    {
        if (!CanLodge(card))
            return;
        
        if (!pile.Insert(card, Entryway == SlotEntryway.Front ? PileInsertionMode.Unshift : PileInsertionMode.Push))
            return;
        
        card.Bind(this);
        
        lodging.OnNext(card);
    }

    public void Release(ICard card)
    {
        if (pile.Remove(card))
            releasing.OnNext(Unit.Default);
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
        return bounds.Contains(worldPoint);
    }

    protected abstract bool CanLodge(ICard card);
    
    protected abstract bool CanLodge(ISlot fromSlot);
}