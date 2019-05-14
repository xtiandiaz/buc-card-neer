using System;
using UniRx;
using UnityEngine;
using Zenject;

public interface ICardController
{
}

public abstract class CardController : ICardController, IDisposable
{
    protected readonly CompositeDisposable disposables = new CompositeDisposable();
    
    private readonly ICard model;
    private readonly ICardView view;
    private readonly GameCamera gameCamera;

    [Inject] private Viewport viewport;
    [Inject] private BoardLayoutSettings layoutSettings;
    private IDisposable destructionDisposable;

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
        view.Position = Vector2.up * (viewport.Size.y + layoutSettings.CardSize.y) * 0.5f; // Dealing position
        
        disposables.Add(model.WhenVisibilityChanged.Subscribe(view.ToggleVisibility));
        disposables.Add(model.WhenFaceChanged.Subscribe(face => view.Flip(face, false)));
        disposables.Add(model.WhenFlipped.Subscribe(face => view.Flip(face, true)));
        
        disposables.Add(model.Worth.Subscribe(value => 
        {
            view.Value = value;

            if (value <= 0)
                model.Destroy();
        }));    

        #region Binding & Arrangement

        disposables.Add(model.WhenBound.Subscribe(view.SetParent));
        
        disposables.Add(model.WhenArranged
            .SelectMany(_ => view.MoveLocalAsObservable(model.LocalPosition))
            .Subscribe(_ => SetViewOrder()));

        #endregion

        #region Interaction

        disposables.Add(model.WhenPicked.Subscribe(_ => 
        {
            view.OnPicked();
            view.SortingOrder = layoutSettings.FloatingCardSortingOrder;
        }));
        
        disposables.Add(model.WhenDragged.Subscribe(_ => view.LocalPosition = model.LocalPosition));
        
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

        #region Destruction

        destructionDisposable = model.WhenDestroyed
            .Do(_ => disposables.Clear())
            .ContinueWith(view.FadeAsObservable(0))
            .Subscribe(_ => 
            {
                view.Destroy();
                Dispose();
            });

        #endregion
    }

    public void Dispose()
    {
        disposables?.Dispose();
        destructionDisposable?.Dispose();
    }

    private void SetViewOrder()
    {
        view.SortingOrder = -model.Index;
    }
}

