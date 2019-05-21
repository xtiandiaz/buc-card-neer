using System;
using UniRx;
using UnityEngine;

[Flags]
public enum SlotType
{
    Supply     = 1 << 0,
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
    bool IsLocked { get; }
    bool IsEmpty { get; }
    SlotType Type { get; }
    Vector3 Position { get; }

    IObservable<ICard> WhenPicked { get; }
    IObservable<ICard> WhenLodged { get; }
    IObservable<Unit> WhenReleased { get; }
    IObservable<Unit> WhenEmptied { get; }
    IObservable<bool> WhenToggledHighlighting { get; }
    IObservable<bool> Locking { get; }

    ICard Pick();
    bool CanMatch(ICard withCard, ISlot fromSlot);
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
    protected readonly ISlotSettings settings;
    
    private readonly IPile pile;
    private readonly Subject<ICard> picking = new Subject<ICard>();
    private readonly Subject<ICard> lodging = new Subject<ICard>();
    private readonly Subject<Unit> releasing = new Subject<Unit>();
    private readonly Subject<Unit> emptying = new Subject<Unit>();
    private readonly Subject<bool> highlighting = new Subject<bool>();
    private readonly ReactiveProperty<bool> isLocked = new ReactiveProperty<bool>(false);
    private Bounds bounds;
    private ICardProvider cardProvider;
    
    protected Slot(IPile pile, ISlotSettings settings, Bounds bounds, Transform transformBond)
    {
        this.pile = pile;
        this.settings = settings;
        this.bounds = bounds;
        
        TransformBond = transformBond;
        IsLocked = settings.ShouldStartLocked;
    }

    public SlotType Type => settings.Type;
    public Vector3 Position => TransformBond.position;
    public Transform TransformBond { get; }
    public bool IsEmpty => pile.Count <= 0;
    
    public bool IsLocked
    {
        get => isLocked.Value;
        private set => isLocked.Value = value;
    }

    public IObservable<ICard> WhenPicked => picking;
    public IObservable<ICard> WhenLodged => lodging;
    public IObservable<Unit> WhenReleased => releasing;
    public IObservable<Unit> WhenEmptied => emptying;
    public IObservable<bool> WhenToggledHighlighting => highlighting.DistinctUntilChanged();
    public IObservable<bool> Locking => isLocked;

    public ICard Pick()
    {
        var card = pile.Peek();
        
        card?.Pick();
        
        if (card != null)
            picking.OnNext(card);

        return card;
    }

    public bool CanMatch(ICard withCard, ISlot fromSlot)
    {
        return CanMatch(withCard) && pile.Peek()?.CanMatch(withCard, fromSlot) == true;
    }
    
    public bool CanLodge(ICard card, ISlot fromSlot)
    {
        return card != null && pile.CanInsert && !pile.DoesContain(card) && CanLodge(fromSlot) && CanLodge(card);
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

        if (!pile.Insert(
            card, 
            settings.Entryway == SlotEntryway.Front ? PileInsertionMode.Unshift : PileInsertionMode.Push,
            settings.DoesSelfArrangeOnLodging))
        {
            Debug.LogError($"[Slot] Couldn't insert {card} in Pile.");
            return;
        }

        OnLodged(card);
    }

    public void Release(ICard card)
    {
        if (pile.Remove(card, settings.DoesSelfArrangeOnReleasing))
            releasing.OnNext(Unit.Default);
        
        if (IsEmpty)
            emptying.OnNext(Unit.Default);
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

    protected abstract bool CanMatch(ICard withCard);
    
    protected abstract bool CanLodge(ICard card);
         
    protected abstract bool CanLodge(ISlot fromSlot);

    protected virtual void OnLodged(ICard card)
    {
        card.Bind(this);
        
        lodging.OnNext(card);
    }
}