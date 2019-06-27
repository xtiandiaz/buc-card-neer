using System;
using UniRx;
using UnityEngine;
using Zenject;

public interface ISlot : IDisposable
{
    SlotType Type { get; }
    
    bool IsLocked { get; }
    bool IsMessy { get; } 
    bool IsEmpty { get; }
    bool HasRoom { get; }
    int Order { get; }

    Vector3 Position { get; }

    IObservable<Unit> WhenDraggingStarted { get; }
    IObservable<Vector3> WhenDragged { get; }
    IObservable<Vector3> WhenDraggingStopped { get; } 

    IObservable<ICard> WhenLodged { get; }
    IObservable<Unit> WhenReleased { get; }
    
    ICard Peek();
    ICard Pop();
    IObservable<Unit> Lodge(ICard card);
    IObservable<Unit> Arrange();
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
    
    private readonly Subject<ICard> lodging = new Subject<ICard>();
    private readonly Subject<Unit> releasing = new Subject<Unit>();
    private readonly ReactiveProperty<bool> isLocked = new ReactiveProperty<bool>(false);
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    private readonly ICardHeap heap;
    
    private readonly ISlotView view;
    private readonly CardArrangementModel arrangementModel;

    protected Slot(ISlotModel model, ISlotView view)
    {
        Type = model.Type;
        IsLocked = model.ShouldStartLocked;
        Order = model.Order;

        heap = (Type & SlotType.Supply) != 0
            ? new CardQueue(model.Capacity) as ICardHeap
            : new CardStack();

        arrangementModel = model.Arrangement;

        this.view = view;
    }

    public SlotType Type { get; }

    public bool IsLocked
    {
        get => isLocked.Value;
        private set => isLocked.Value = value;
    }

    public bool IsMessy { get; private set; }
    public bool IsEmpty => heap.Count <= 0;
    public bool HasRoom => heap.HasRoom;
    public int Order { get; }

    public Vector3 Position => view.Transform.position;

    public IObservable<Unit> WhenDraggingStarted => view.WhenDraggingStarted;
    public IObservable<Vector3> WhenDragged => view.WhenDragged;
    public IObservable<Vector3> WhenDraggingStopped => view.WhenDraggingStopped;
    
    public IObservable<ICard> WhenLodged => lodging;
    public IObservable<Unit> WhenReleased => releasing;

    public ICard Peek()
    {
        return heap.Peek();
    }
    
    public ICard Pop()
    {
        var poppedCard = heap.Pop();

        IsMessy = poppedCard != null;
        
        return poppedCard;
    }
    
    public IObservable<Unit> Lodge(ICard card)
    {
        return Observable.Create<Unit>(observer =>
            {
                var cardIndex = heap.Insert(card);
                if (!cardIndex.HasValue)
                {
                    observer.OnError(new Exception($"[Slot] Couldn't insert {card} in Pile."));
                    
                    return Disposable.Empty;
                }

                card.SetParent(view.Transform);

                return Arrange()
                    .Subscribe(observer);
            })
            .Do(_ => lodging.OnNext(card))
            .DoOnError(Debug.LogException);
    }

    public IObservable<Unit> Arrange()
    {
        return heap.Map((card, index) => card.Arrange(arrangementModel.GetArrangementForIndex(index)))
            .Merge()
            .AsSingleUnitObservable()
            .DoOnCompleted(() => IsMessy = false);
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
        return heap.DoesContain(card);
    }

    public bool DoesContain(Vector3 position)
    {
        return view.Bounds.Contains(position);
    }

    public void Dispose()
    {
        lodging?.Dispose();
        releasing?.Dispose();
        isLocked?.Dispose();
        disposables?.Dispose();
    }
}