using System;
using DG.Tweening;
using UniRx;
using UnityEngine;
using Zenject;

public interface ICardController : IInitializable, IDisposable
{
}

public abstract class CardController : ICardController
{
    protected readonly CompositeDisposable disposables = new CompositeDisposable();
    private readonly CompositeDisposable lateDisposables = new CompositeDisposable();
    
    private static readonly TimeSpan ClashTiltTimeSpan = TimeSpan.FromSeconds(0.25f);
    private readonly ICard model;
    private readonly ICardView view;
    private readonly GameCamera gameCamera;

    [Inject] private Viewport viewport;
    [Inject] private BoardLayoutSettings layoutSettings;

    protected CardController(ICard model, ICardView view)
    {
        this.model = model;
        this.view = view;
    }
    
    [Inject]
    public virtual void Initialize()
    {
        view.FrontFace = model.FrontFace;
        view.BackFace = model.BackFace;
        view.Position = (viewport.Size.y + layoutSettings.CardSize.y) * 0.5f * Vector2.up; // Dealing position

        #region Transform

        disposables.Add(model.WhenMoved
            .Subscribe(_ => view.MoveLocal(model.LocalPosition)));
        
        disposables.Add(model.WhenFlipped
            .Subscribe(face => view.Flip(face, true)));

        #endregion
        
        #region Content

        disposables.Add(model.ValueAsObservable
            .Subscribe(value =>
            {
                view.Value = value;

                if (value <= 0)
                    model.Destroy();
            }));
        
        #endregion

        #region Arrangement

        disposables.Add(model.WhenArranged
            .Subscribe(_ =>
            {
                view.MoveLocal(model.ArrangedPosition);
                SortView();
            }));

        #endregion

        #region Binding

        disposables.Add(model.WhenBound
            .Subscribe(view.SetParent));
        
        #endregion

        #region Interaction

        disposables.Add(model.WhenPicked
            .Subscribe(_ => 
            {
                view.Pick();
                view.SortingOrder = layoutSettings.FloatingCardSortingOrder;
            }));
        
        disposables.Add(model.WhenDragged
            .Subscribe(_ => view.LocalPosition = model.LocalPosition));
        
        disposables.Add(model.WhenDropped
            .Subscribe(_ =>
            { 
                view.Drop(model.ArrangedPosition)
                    .OnComplete(SortView);
            }));

        #endregion

        #region Effects

        disposables.Add(model.WhenFaded
            .Subscribe(view.Fade));
        
        disposables.Add(model.WhenTinted
            .Subscribe(withColorByFactor => 
                view.Tint(withColorByFactor.Item1, withColorByFactor.Item2)));
        
        disposables.Add(model.WhenFogged
            .Subscribe(withColorByFactor => 
                view.Fog(withColorByFactor.Item1, withColorByFactor.Item2)));

        #endregion

        #region Clashing

        disposables.Add(model.WhenClashed
            .Subscribe(withDirection => view.Tilt(withDirection, ClashTiltTimeSpan)));
        
        #endregion

        #region Impacting

        lateDisposables.Add(model.WhenImpacted
            .Subscribe(_ => view.Spin(2)));

        #endregion

        #region Destruction

        lateDisposables.Add(model.WhenDestroyed
            .Do(_ =>
            {
                disposables.Clear();
                view.KillMove();
            })
            .ContinueWith(view.FadeAsObservable(0))
            .Subscribe(
                _ => 
                {
                    view.Destroy();
                    Dispose();
                },
                e => Debug.LogError(e.Message)));

        #endregion
    }

    public void Dispose()
    {
        disposables.Dispose();
        lateDisposables.Dispose();
    }

    private void SortView()
    {
        view.SortingOrder = -model.Index * 10;
    }
}

