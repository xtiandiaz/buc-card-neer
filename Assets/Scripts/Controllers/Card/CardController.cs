using System;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public interface ICardController : IInitializable, IDisposable
{
}

public abstract class CardController : ICardController
{
    protected readonly CompositeDisposable disposables = new CompositeDisposable();
    
    private readonly ICard model;
    private readonly ICardView view;

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
        var dealingPosition = (viewport.Size.y + layoutSettings.CardSize.y) * 0.5f * Vector2.up;
        
        view.Position = dealingPosition;

        #region Transform
        
        disposables.Add(model.WhenBounced
            .Subscribe(_ => 
            {
                view.MoveLocal(dealingPosition + Vector2.down * layoutSettings.CardSize.y);
                view.Rotate(Random.Range(5f, 15f) * (Random.value < 0.5f ? -1f : 1f) * Vector3.forward);
            }));

        #endregion

        #region Destruction

        disposables.Add(model.WhenDestroyed
            .Subscribe(_ => Dispose()));

        #endregion
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}

