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

public interface ISlot : ICardBond, ICardConsumer
{
    bool IsVisible { get; set; }
    bool IsLocked { get; set; }
    SlotType Type { get; }
    SlotEntryway Entryway { get; set; }
    Vector3 Position { get; }

    IObservable<ICard> WhenPicked { get; }
    IObservable<ICard> WhenLodged { get; }
    IObservable<Unit> WhenReleased { get; }
    IObservable<bool> Highlighting { get; }
    IObservable<bool> Visibility { get; }
    IObservable<bool> Locking { get; }

    ICard Pick();
    bool CanMatch(ICard card, ISlot fromSlot);
    void Match(ICard card);
    bool CanLodge(ICard card, ISlot fromSlot);
    void Lodge(ICard card);
    void Arrange();
    void ToggleHighlight(bool on);
    bool DoesContain(Vector3 worldPoint);
}

public interface ICardBond
{
    Transform TransformBond { get; }
    
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
    private ICardProvider cardProvider;
    
    protected Slot(SlotType type, IPile pile, Transform transform, Bounds bounds)
    {
        this.pile = pile;
        this.bounds = bounds;
        
        Type = type;
        TransformBond = transform;
    }

    public SlotType Type { get; }
    public SlotEntryway Entryway { get; set; }
    public Vector3 Position => TransformBond.position;
    public Transform TransformBond { get; }
    
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

    public bool CanMatch(ICard card, ISlot fromSlot)
    {
        return pile.Peek()?.CanMatch(card, fromSlot) == true;
    }

    public void Match(ICard card)
    {
        pile.Peek()?.Match(card);
    }

    public void Lodge(ICard card)
    {
        if (!CanLodge(card))
        {
            Debug.LogWarning($"[Slot] Can't lodge {card}.");
            return;
        }

        if (!pile.Insert(card, Entryway == SlotEntryway.Front ? PileInsertionMode.Unshift : PileInsertionMode.Push))
        {
            Debug.LogError($"[Slot] Couldn't insert {card} in Pile.");
            return;
        }
        
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
    
    public void SetProvider(ICardProvider provider)
    {
        cardProvider = provider;
    }

    public void Consume(int count)
    {
        if (cardProvider.IsExhausted)
            return;

        Lodge(cardProvider.Provide());
    }
}