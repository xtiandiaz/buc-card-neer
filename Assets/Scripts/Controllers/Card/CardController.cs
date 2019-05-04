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

        disposables.Add(model.Arranging.Subscribe(_ =>
        {
            view.Move(model.Position);
            view.SortingOrder = -model.IndexInSlot;
        }));
        
        disposables.Add(model.Visibility.Subscribe(view.ToggleVisibility));
        disposables.Add(model.Fading.Subscribe(view.Fade));
        disposables.Add(model.Tinting.Subscribe(withColorByFactor =>
            view.Tint(withColorByFactor.Item1, withColorByFactor.Item2)));
        disposables.Add(model.Fogging.Subscribe(withColorByFactor =>
            view.Fog(withColorByFactor.Item1, withColorByFactor.Item2)));
        
        view.Flip(model.Face, false);
        disposables.Add(model.Facing.Skip(1).Subscribe(face => view.Flip(face, true)));
        
        disposables.Add(model.Picking.Subscribe(_ => view.OnPicked()));
        disposables.Add(model.Dragging.Subscribe(_ => view.Position = model.Position));
        disposables.Add(model.Dropping.Subscribe(_ => 
        {
            view.OnDropped();
            view.Move(model.Position);
        }));
    }

    public virtual void Dispose()
    {
        disposables?.Dispose();
    }
}

