using System;
using Zenject;
using UniRx;
using UnityEngine;

public interface IBoardController : IInitializable, IDisposable
{
}

public class BoardController : IBoardController
{
    public class Factory : PlaceholderFactory<IBoard, IBoardView, BoardController>
    {
    }
    
    private static readonly TimeSpan ShootingImpactDelay = TimeSpan.FromSeconds(0.25f);
    private static readonly TimeSpan CollectionDelay = TimeSpan.FromSeconds(0.5f);
    
    private readonly IBoard model;
    private readonly IBoardView view;
    private readonly ISea sea;
    private readonly IShip ship;
    private readonly IMoveListener moveListener;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private BoardController(
        IBoard model, 
        IBoardView view,
        ISea sea,
        IShip ship,
        IMoveListener moveListener
        )
    {
        this.model = model;
        this.view = view;
        this.sea = sea;
        this.ship = ship;
        this.moveListener = moveListener;
    }

    [Inject]
    public void Initialize()
    {
        disposables.Add(moveListener.WhenMoved
            .Subscribe(_ =>
            {
                Debug.Log("Player Moved!");
                
                sea.Clash();
            }));

        #region Battling

        disposables.Add(ship.WhenArmed.Subscribe(_ => sea.Lock()));
        
        disposables.Add(ship.WhenShot
            .Delay(ShootingImpactDelay)
            .Do(sea.Impact)
            .Delay(CollectionDelay)
            .Do(_ => sea.Collect())
            .Subscribe(_ =>
            {
                sea.Unlock();
                ship.Unlock();
            }));

        #endregion

        #region Automatic Collection & Storing

        disposables.Add(sea.WhenCollected.Subscribe(resource =>
        {
            resource.IsBoarded = true;
            
            ship.Store(resource);
        }));

        #endregion
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}