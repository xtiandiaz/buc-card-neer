using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public interface ICardController
{
    void Initialize();
}

public abstract class CardController : ICardController, IDisposable
{
    private const float CardMoveDurationInSeconds = 0.5f;
    private const float CardReturnDurationInSeconds = 0.25f;
    
    private readonly ICard model;
    private readonly ICardView view;
    private readonly BoardCamera boardCamera;
    private readonly ObservableEventTrigger eventTrigger;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    protected CardController(ICard model, ICardView view)
    {
        this.model = model;
        this.view = view;
    }

    public virtual void Initialize()
    {
        view.FrontFace = model.FrontFace;
        view.BackFace = model.BackFace;
        
        disposables.Add(model.ChangedLocalPosition.Subscribe(localPosition => view.LocalPosition = localPosition));
        disposables.Add(model.BecameVisible.Subscribe(view.ToggleVisibility));
        
        view.Flip(model.Face, false);
        disposables.Add(model.ChangedFace.Skip(1).Subscribe(face => view.Flip(face, true)));
        
        disposables.Add(
            model.Lodged.Subscribe(transform =>
            {    
                view.SetParent(transform);
                view.SetLocalPosition(model.LocalPosition, CardReturnDurationInSeconds);
            }));
        
        disposables.Add(model.Picked.Subscribe(_ => view.OnPicked()));
        disposables.Add(model.Dropped.Subscribe(_ =>
            {
                view.OnDropped();
                view.SetLocalPosition(Vector3.zero, CardReturnDurationInSeconds);
            }));
        
        disposables.Add(view.DragStarted.Subscribe(_ => model.Pick()));
        disposables.Add(view.Dragged.Subscribe(worldPositionDelta => view.LocalPosition += worldPositionDelta));
        disposables.Add(view.DragEnded.Subscribe(dropPosition => model.Drop(dropPosition)));
        
        view.ToggleDragging(true);
    }

    public virtual void Dispose()
    {
        disposables?.Dispose();
        view.Destroy();
    }
}

