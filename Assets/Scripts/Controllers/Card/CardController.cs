using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public interface ICardController
{
}

public abstract class CardController : ICardController, IDisposable
{
    private readonly ICard model;
    private readonly ICardView view;
    private readonly BoardCamera boardCamera;
    private readonly ObservableEventTrigger eventTrigger;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    [Inject] private Viewport viewport;
    [Inject] private BoardLayoutSettings layoutSettings;

    protected CardController(ICard model, ICardView view)
    {
        this.model = model;
        this.view = view;
    }
    
    [Inject]
    protected virtual void Initialize()
    {
        view.FrontFace = model.FrontFace;
        view.BackFace = model.BackFace;
        view.Value = model.Value;
        view.Position = Vector2.up * (viewport.Size.y + layoutSettings.CardSize.y) * 0.5f;

        disposables.Add(model.WhenArranged.Subscribe(_ =>
        {
            view.Move(model.Position);
            view.SortingOrder = -model.IndexInSlot;
        }));
        
        disposables.Add(model.Visibility.Subscribe(view.ToggleVisibility));
        
        disposables.Add(model.Facing.Subscribe(face => view.Flip(face, false)));
        disposables.Add(model.Flipping.Subscribe(face => view.Flip(face, true)));
        
        disposables.Add(model.Worth.Subscribe(value =>
        {
            view.Value = value;

            if (value <= 0)
                model.Destroy();
        }));
        
        disposables.Add(model.Destruction.Subscribe(_ =>
        {
            view.Destroy();
            Dispose();
        }));

        #region Interaction

        disposables.Add(model.Picking.Subscribe(_ => view.OnPicked()));
        
        disposables.Add(model.Dragging.Subscribe(_ => view.Position = model.Position));
        
        disposables.Add(model.Dropping.Subscribe(_ => 
        {
            view.OnDropped();
            view.Move(model.Position);
        }));

        #endregion

        #region Effects

        disposables.Add(model.Fading.Subscribe(view.Fade));
        
        disposables.Add(model.Tinting.Subscribe(withColorByFactor => 
            view.Tint(withColorByFactor.Item1, withColorByFactor.Item2)));
        
        disposables.Add(model.Fogging.Subscribe(withColorByFactor =>
            view.Fog(withColorByFactor.Item1, withColorByFactor.Item2)));

        #endregion
    }

    public virtual void Dispose()
    {
        disposables?.Dispose();
    }
}

