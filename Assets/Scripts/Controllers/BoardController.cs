using System;
using UniRx;
using Zenject;

public interface IBoardController : IInitializable, IDisposable
{
}

public class BoardController : IBoardController
{
    private readonly ISea sea;
    private readonly IShip ship;
    private readonly ISupplyController supplyController;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private BoardController(
        ISea sea,
        IShip ship,
        ISupplyController supplyController
        )
    {
        this.sea = sea;
        this.ship = ship;
        this.supplyController = supplyController;
    }

    public void Initialize()
    {
        disposables.Add(ship.WhenCardBoarded
            .Take(1)
            .Do(_ => sea.Lock())
            .ContinueWith(supplyController.WhenRoundCompleted
                .Take(1)
                .Do(_ => sea.Unlock()))
            .RepeatSafe()
            .Subscribe());
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}