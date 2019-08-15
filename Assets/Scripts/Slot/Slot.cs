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
    int Index { get; }
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

    IObservable<Vector2> WhenPressed { get; }
    IObservable<Unit> WhenUnpressed { get; }
    IObservable<Unit> WhenDraggingStarted { get; }
    IObservable<Vector2> WhenDragged { get; }
    IObservable<Vector3> WhenDraggingStopped { get; } 

    IObservable<ICard> WhenLodged { get; }
    IObservable<ICard> WhenReleased { get; }

    ICard Peek();
    ICard Pop();

    IObservable<Unit> Lodge(
        ICard card,
        SlotLodgingMode withMode = SlotLodgingMode.Systematic,
        bool andShouldRearrangeOthers = false);
    
    IObservable<Unit> ArrangeAsObservable();
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
    
    private readonly ISlotView view;
    private readonly CardArrangementModel arrangementModel;
    private readonly bool shouldSelfArrange;
    private readonly int index;
    
    private readonly Subject<ICard> lodging = new Subject<ICard>();
    private readonly Subject<ICard> releasing = new Subject<ICard>();
    private readonly ReactiveProperty<bool> isLocked = new ReactiveProperty<bool>(false);
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    protected Slot(ISlotModel model, ISlotView view)
    {
        Type = model.Type;
        index = model.Index;
        IsLocked = model.ShouldStartLocked;
        shouldSelfArrange = model.ShouldSelfArrange;

        pile = (Type & SlotType.Supply) != 0 || model.Capacity > 0
            ? new Pile(model.Capacity)
            : new Pile();

        arrangementModel = model.Arrangement;

        this.view = view;
    }

    int ICardBond.Index => index;
    Transform ICardBond.Transform => view.Transform;

    void ICardBond.Release(ICard card)
    {
        IsMessy |= pile.Remove(card);
        
        releasing.OnNext(card);
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

    public IObservable<Vector2> WhenPressed => view.WhenPressed;
    public IObservable<Unit> WhenUnpressed => view.WhenReleased;
    public IObservable<Unit> WhenDraggingStarted => view.WhenDraggingStarted;
    public IObservable<Vector2> WhenDragged => view.WhenDragged;
    public IObservable<Vector3> WhenDraggingStopped => view.WhenDraggingStopped;
    
    public IObservable<ICard> WhenLodged => lodging;
    public IObservable<ICard> WhenReleased => releasing.DistinctUntilChanged();

    public ICard Peek()
    {
        return pile.Peek();
    }
    
    public ICard Pop()
    {
        var poppedCard = pile.Pop();

        IsMessy |= poppedCard != null;
        
        releasing.OnNext(poppedCard);
        
        return poppedCard;
    }
    
    public IObservable<Unit> Lodge(
        ICard card, 
        SlotLodgingMode withMode = SlotLodgingMode.Systematic, 
        bool andShouldRearrangeOthers = false)
    {
        return Observable.Create<Unit>(observer =>
            {
                var newIndex = pile.Insert(card);
                if (!newIndex.HasValue)
                {
                    observer.OnError(new Exception($"[Slot] Couldn't insert {card} in Pile."));
                    
                    return Disposable.Empty;
                }

                IsMessy = !andShouldRearrangeOthers;
                
                if (andShouldRearrangeOthers)
                {
                    pile.ForEach((otherCard, index) =>
                    {
                        if (index != newIndex.Value)
                            otherCard.Arrange(arrangementModel.GetArrangementForIndex(index, pile.Extent));
                    });
                }

                return card.Lodge(
                        this,
                        arrangementModel.GetArrangementForIndex(newIndex.Value, pile.Extent),
                        withMode == SlotLodgingMode.Systematic ? CardArrangementMode.Normal : CardArrangementMode.Fast)
                    .Subscribe(observer);
            })
            .Do(_ => lodging.OnNext(card))
            .DoOnError(Debug.LogException);
    }

    public IObservable<Unit> ArrangeAsObservable()
    {
        return pile.Map((card, index) => 
                card.ArrangeAsObservable(arrangementModel.GetArrangementForIndex(index, pile.Extent)))
            .Merge()
            .AsSingleUnitObservable()
            .DoOnCompleted(() => IsMessy = false);
    }

    public IObservable<Unit> ConditionallyArrange()
    {
        return shouldSelfArrange
            ? ArrangeAsObservable()
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
        lodging.Dispose();
        releasing.Dispose();
        isLocked.Dispose();
        
        disposables.Dispose();
    }
}