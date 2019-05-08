using System;
using UniRx;
using UnityEngine;
using Zenject;

public interface ICardController
{
}

public abstract class CardController : ICardController, IDisposable
{
    private readonly ICard model;
    private readonly ICardView view;
    private readonly BoardCamera boardCamera;
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
        
        disposables.Add(model.WhenVisibilityChanged.Subscribe(view.ToggleVisibility));
        disposables.Add(model.WhenFaceChanged.Subscribe(face => view.Flip(face, false)));
        disposables.Add(model.WhenFlipped.Subscribe(face => view.Flip(face, true)));
        
        disposables.Add(model.WhenValueChanged.Subscribe(value => 
        {
            view.Value = value;

            if (value <= 0)
                model.Destroy();
        }));
        
        disposables.Add(model.WhenArranged
            .SelectMany(_ => view.MoveAsObservable(model.Position))
            .Subscribe(_ => SetViewOrder()));
        
        disposables.Add(model.WhenDestroyed.Subscribe(_ => 
        {
            view.Destroy();
            Dispose();
        }));

        #region Interaction

        disposables.Add(model.WhenPicked.Subscribe(_ => 
        {
            view.OnPicked();
            view.SortingOrder = layoutSettings.FloatingCardSortingOrder;
        }));
        
        disposables.Add(model.WhenDragged.Subscribe(toPosition => view.Position = toPosition));
        
        disposables.Add(model.WhenDropped
            .SelectMany(_ => view.OnDropped())
            .Subscribe(_ => SetViewOrder()));

        #endregion

        #region Effects

        disposables.Add(model.WhenFaded.Subscribe(view.Fade));
        
        disposables.Add(model.WhenTinted.Subscribe(withColorByFactor => 
            view.Tint(withColorByFactor.Item1, withColorByFactor.Item2)));
        
        disposables.Add(model.WhenFogged.Subscribe(withColorByFactor =>
            view.Fog(withColorByFactor.Item1, withColorByFactor.Item2)));

        #endregion
    }

    public virtual void Dispose()
    {
        disposables?.Dispose();
    }

    private void SetViewOrder()
    {
        view.SortingOrder = -model.Index;
    }
}

