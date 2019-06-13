using System;
using Zenject;
using UniRx;

public interface IBoardController : IInitializable, IDisposable
{
}

public class BoardController : IBoardController
{
    public class Factory : PlaceholderFactory<IBoard, IBoardView, BoardController>
    {
    }
    
    private static readonly TimeSpan ImpactDuration = TimeSpan.FromSeconds(0.5);

    private readonly IBoard model;
    private readonly IBoardView view;
    private readonly ISea sea;
    private readonly IShip ship;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private BoardController(
        IBoard model, 
        IBoardView view,
        ISea sea,
        IShip ship
    )
    {
        this.model = model;
        this.view = view;
        this.sea = sea;
        this.ship = ship;
    }

    [Inject]
    public void Initialize()
    {
        #region Boarding
        
        disposables.Add(ship.WhenBoarded
            .Merge(ship.WhenMatched.Delay(TimeSpan.FromSeconds(0.3)))
            .SelectMany(_ => sea.Clash())
            .Do(_ => OnClashed())
            .SelectMany(_ => sea.Resupply())
            .Subscribe());
        
        #endregion

        #region Battling

        disposables.Add(ship.WhenShot
            .Do(shotValue =>
            {
                sea.Lock();
                sea.Impact(shotValue);
            })
            .Delay(ImpactDuration)
            .SelectMany(_ => sea
                .Collect()
                .Select(ship.BoardAndStore)
                .Concat()
                .LastOrDefault())
            .Do(_ => ship.Unlock())
            .SelectMany(_ => sea.Clash())
            .Do(_ => OnClashed())
            .SelectMany(_ => sea.Resupply())
            .Subscribe());

        #endregion
    }

    public void Dispose()
    {
        disposables.Dispose();
    }

    private void OnClashed()
    {
        sea.Arrange();
        sea.Unlock();
    }
}