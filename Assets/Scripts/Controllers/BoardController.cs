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
    
    private static readonly TimeSpan ShotDelay = TimeSpan.FromSeconds(0.25);
    private static readonly TimeSpan ImpactDelay = TimeSpan.FromSeconds(0.25);
    private static readonly TimeSpan CollectionDelay = TimeSpan.FromSeconds(0.5);
    
    private readonly IBoard model;
    private readonly IBoardView view;
    private readonly ISea sea;
    private readonly IShip ship;
    private readonly IMoveRouter moveRouter;
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    private readonly Subject<Unit> clashing = new Subject<Unit>();

    private BoardController(
        IBoard model, 
        IBoardView view,
        ISea sea,
        IShip ship,
        IMoveRouter moveRouter
        )
    {
        this.model = model;
        this.view = view;
        this.sea = sea;
        this.ship = ship;
        this.moveRouter = moveRouter;
    }

    [Inject]
    public void Initialize()
    {
        #region Clashing
        
        disposables.Add(moveRouter.WhenPlayerMoved
            .Subscribe(clashing));
        
        disposables.Add(clashing
            .Subscribe(_ => sea.Clash()));

        #endregion

        #region Battling

        disposables.Add(ship.WhenArmed
            .Subscribe(_ => sea.Lock()));
        
        disposables.Add(ship.WhenShot
            .Delay(ShotDelay)
            .Do(sea.Impact)
            .Delay(ImpactDelay)
            .Do(_ => sea.Collect())
            .Delay(CollectionDelay)
            .Do(_ => ship.Unlock())
            .AsUnitObservable()
            .Subscribe(clashing));

        #endregion

        #region Looting

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