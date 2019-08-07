using System;
using UniRx;
using UnityEngine;
using Zenject;

public enum SlotLodgingMode
{
    Systematic,
    Voluntary
}

public interface ICardBond
{
    Transform Transform { get; }
    
    void Release(ICard card);
}

public interface ISlot : ICardBond, IDisposable
{
    SlotType Type { get; }
    
    bool IsLocked { get; }
    bool IsMessy { get; } 
    bool IsEmpty { get; }
    bool HasRoom { get; }

    Vector3 Position { get; }

    IObservable<Unit> WhenPressed { get; }
    IObservable<Unit> WhenReleased { get; }
    IObservable<Unit> WhenDraggingStarted { get; }
    IObservable<Vector3> WhenDragged { get; }
    IObservable<Vector3> WhenDraggingStopped { get; } 

    IObservable<ICard> WhenLodged { get; }

    ICard Peek();
    ICard Pop();
    IObservable<Unit> Lodge(ICard card, SlotLodgingMode withMode = SlotLodgingMode.Systematic);
    IObservable<Unit> Arrange(SlotLodgingMode withLodgingMode = SlotLodgingMode.Systematic);
    IObservable<Unit> ConditionallyArrange();
    void Lock();
    void Unlock();
    void ToggleHighlight(bool on);
    
    bool DoesContain(ICard card);
    bool DoesContain(Vector3 position);
}

public class Slot : ISlot
{
    public class Factory : PlaceholderFactory<ISlotModel, ISlotView, Slot>
    {
    }
    
    protected readonly IPile pile;
    
    private readonly Subject<ICard> lodging = new Subject<ICard>();
    private readonly ReactiveProperty<bool> isLocked = new ReactiveProperty<bool>(false);
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    private readonly ISlotView view;
    private readonly CardArrangementModel arrangementModel;
    private readonly bool shouldSelfArrange;

    protected Slot(ISlotModel model, ISlotView view)
    {
        Type = model.Type;
        IsLocked = model.ShouldStartLocked;
        shouldSelfArrange = model.ShouldSelfArrange;

        pile = (Type & SlotType.Supply) != 0 || model.Capacity > 0
            ? new Pile(model.Capacity)
            : new Pile();

        arrangementModel = model.Arrangement;

        this.view = view;
    }

    Transform ICardBond.Transform => view.Transform;

    void ICardBond.Release(ICard card)
    {
        IsMessy |= pile.Remove(card);
    }

    public SlotType Type { get; }

    public bool IsLocked
    {
        get => isLocked.Value;
        private set => isLocked.Value = value;
    }

    public bool IsMessy { get; private set; }
    public bool IsEmpty => pile.Count <= 0;
    public bool HasRoom => pile.HasRoom;

    public Vector3 Position => view.Transform.position;

    public IObservable<Unit> WhenPressed => view.WhenPressed;
    public IObservable<Unit> WhenReleased => view.WhenReleased;
    public IObservable<Unit> WhenDraggingStarted => view.WhenDraggingStarted;
    public IObservable<Vector3> WhenDragged => view.WhenDragged;
    public IObservable<Vector3> WhenDraggingStopped => view.WhenDraggingStopped;
    
    public IObservable<ICard> WhenLodged => lodging;

    public ICard Peek()
    {
        return pile.Peek();
    }
    
    public ICard Pop()
    {
        var poppedCard = pile.Pop();

        IsMessy |= poppedCard != null;
        
        return poppedCard;
    }
    
    public IObservable<Unit> Lodge(ICard card, SlotLodgingMode withMode = SlotLodgingMode.Systematic)
    {
        return Observable.Create<Unit>(observer =>
            {
                if (!pile.Insert(card))
                {
                    observer.OnError(new Exception($"[Slot] Couldn't insert {card} in Pile."));
                    
                    return Disposable.Empty;
                }

                card.Bind(this);

                IsMessy = true;

                return Arrange(withMode)
                    .Subscribe(observer);
            })
            .Do(_ => lodging.OnNext(card))
            .DoOnError(Debug.LogException);
    }

    public IObservable<Unit> Arrange(SlotLodgingMode withLodgingMode = SlotLodgingMode.Systematic)
    {
        return pile.Map((card, index) => card.Arrange(
                arrangementModel.GetArrangementForIndex(index, pile.Extent, withLodgingMode)))
            .Merge()
            .AsSingleUnitObservable()
            .DoOnCompleted(() => IsMessy = false);
    }

    public IObservable<Unit> ConditionallyArrange()
    {
        return shouldSelfArrange
            ? Arrange()
            : Observable.Empty<Unit>();
    }

    public void Lock()
    {
        IsLocked = true;
    }

    public void Unlock()
    {
        IsLocked = false;
    }

    public void ToggleHighlight(bool toValue)
    {
        view.ToggleHighlight(toValue);
    }

    public bool DoesContain(ICard card)
    {
        return pile.DoesContain(card);
    }

    public bool DoesContain(Vector3 position)
    {
        return view.Bounds.Contains(position);
    }

    public void Dispose()
    {
        lodging?.Dispose();
        isLocked?.Dispose();
        disposables?.Dispose();
    }
}