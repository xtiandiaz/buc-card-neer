using System;
using UniRx;
using UnityEngine;

public enum LodgingFace
{
    Current,
    Front = CardFace.Front,
    Back = CardFace.Back
}

public interface ISlot : ICardBond, ICardConsumer
{
    bool IsLocked { get; }
    bool IsEmpty { get; }
    SlotType Type { get; }
    Vector3 Position { get; }
    ISlotSettings Settings { get; }

    IObservable<ICard> WhenPicked { get; }
    IObservable<ICard> WhenLodged { get; }
    IObservable<ICard> WhenMatched { get; }
    IObservable<Unit> WhenReleased { get; }
    IObservable<bool> WhenToggledHighlighting { get; }

    ICard Peek();
    ICard Pick();
    bool CanDefer(ICard card);
    IObservable<Unit> Defer(ICard card);
    bool CanMatch(ICard withCard, ISlot fromSlot);
    void Match(ICard card);
    bool CanLodge(ICard card, ISlot fromSlot);
    void Lodge(ICard card);
    void KnockOut();
    void Arrange();
    void Lock();
    void Unlock();
    void ToggleHighlight(bool on);
    bool DoesContain(Vector3 worldPoint);
}

public abstract class Slot : ISlot
{
    protected readonly IPile pile;
    
    private readonly Subject<ICard> picking = new Subject<ICard>();
    private readonly Subject<ICard> lodging = new Subject<ICard>();
    private readonly Subject<ICard> matching = new Subject<ICard>();
    private readonly Subject<Unit> releasing = new Subject<Unit>();
    private readonly Subject<bool> highlighting = new Subject<bool>();
    private readonly ReactiveProperty<bool> isLocked = new ReactiveProperty<bool>(false);
    private Bounds bounds;
    private ICardProvider cardProvider;
    private int? provisionCapacity;
    
    protected Slot(IPile pile, ISlotSettings settings, Bounds bounds, Transform transformBond)
    {
        this.pile = pile;
        this.bounds = bounds;
        
        Settings = settings;
        TransformBond = transformBond;
        IsLocked = settings.ShouldStartLocked;
    }

    public SlotType Type => Settings.Type;
    public Vector3 Position => TransformBond.position;
    public Transform TransformBond { get; }
    public bool IsEmpty => pile.Count <= 0;
    
    public bool IsLocked
    {
        get => isLocked.Value;
        private set => isLocked.Value = value;
    }
    
    public ISlotSettings Settings { get; }

    public IObservable<ICard> WhenPicked => picking;
    public IObservable<ICard> WhenLodged => lodging;
    public IObservable<ICard> WhenMatched => matching;
    public IObservable<Unit> WhenReleased => releasing;
    public IObservable<bool> WhenToggledHighlighting => highlighting.DistinctUntilChanged();

    protected PileInsertionMode PileInsertionMode => Settings.Entryway == SlotEntryway.Front
        ? PileInsertionMode.Unshift
        : PileInsertionMode.Push;

    public ICard Peek()
    {
        return pile.Peek();
    }

    public ICard Pick()
    {
        var card = pile.Peek();
        
        card?.Pick();
        
        if (card != null)
            picking.OnNext(card);

        return card;
    }

    public abstract bool CanDefer(ICard card);

    public IObservable<Unit> Defer(ICard card)
    {
        return Observable.Create<Unit>(observer =>
            {
                if (!pile.Remove(card))
                    observer.OnError(new Exception("The Card to defer isn't lodged in this Slot."));
                else
                {
                    if (IsEmpty)
                    {
                        if (provisionCapacity.HasValue)
                            ConsumeAsObservable(provisionCapacity.Value - 1, TimeSpan.FromSeconds(0.1))
                                .Subscribe(observer);
                        else
                            observer.OnError(
                                new Exception(
                                    "The Slot must provision itself but no provision capacity has been set."));
                    }
                    
                    card.Bounce();
                    
                    observer.OnCompleted();
                }

                return Disposable.Create(() => { });
            })
            .Delay(TimeSpan.FromSeconds(0.4))
            .DoOnSubscribe(Lock)
            .DoOnCompleted(() =>
            {
                pile.Insert(card, PileInsertionMode);
                
                Arrange();
                Unlock();
            });
    }

    public bool CanMatch(ICard withCard, ISlot fromSlot)
    {
        return CanMatch(withCard) 
               && pile.Peek()?.CanMatch(withCard) == true;
    }
    
    public bool CanLodge(ICard card, ISlot fromSlot)
    {
        return card != null 
               && pile.CanInsert 
               && !pile.DoesContain(card) 
               && CanLodge(fromSlot) 
               && CanLodge(card);
    }

    public void Match(ICard card)
    {
        pile.Peek()?.Match(card);
        
        matching.OnNext(card);
    }

    public virtual void Lodge(ICard card)
    {
        if (!CanLodge(card))
        {
            Debug.LogWarning($"[Slot] Can't lodge {card}.");
            return;
        }

        if (!pile.Insert(card, PileInsertionMode))
        {
            Debug.LogError($"[Slot] Couldn't insert {card} in Pile.");
            return;
        }

        card.Bind(this);

        lodging.OnNext(card);
    }

    public virtual void Release(ICard card)
    {
        if (pile.Remove(card))
            releasing.OnNext(Unit.Default);
    }

    public void KnockOut()
    {
        Peek()?.Destroy();
    }

    public void Arrange()
    {
        pile.Arrange();
    }

    public void Lock()
    {
        IsLocked = true;
    }
    
    public void Unlock()
    {
        IsLocked = false;
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

    public void SetCapacity(int toValue)
    {
        provisionCapacity = toValue;
    }

    public void Consume()
    {
        if (cardProvider.IsExhausted)
            return;

        Lodge(cardProvider.Provide());
    }

    public IObservable<Unit> ConsumeAsObservable(int count, TimeSpan atIntervalsWithSpan)
    {
        return Observable.Timer(TimeSpan.Zero, atIntervalsWithSpan)
            .Take(count)
            .Do(_ => Consume())
            .AsSingleUnitObservable();
    }

    public IObservable<Unit> FillToCapacity(TimeSpan atIntervalsWithSpan)
    {
        return !provisionCapacity.HasValue 
            ? Observable.ReturnUnit() 
            : ConsumeAsObservable(provisionCapacity.Value, atIntervalsWithSpan);
    }

    protected abstract bool CanMatch(ICard withCard);
    
    protected abstract bool CanLodge(ICard card);
         
    protected abstract bool CanLodge(ISlot fromSlot);
}