using System;
using UniRx;
using Zenject;

public interface IVisualEffectsController : IInitializable, IDisposable
{
}

public class VisualEffectsController : IVisualEffectsController
{
    private readonly IGameCamera camera;
    private readonly IGameStatus gameStatus;
    private readonly IShip ship;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private VisualEffectsController(
        IGameCamera camera,
        IGameStatus gameStatus,
        IShip ship
    )
    {
        this.camera = camera;
        this.gameStatus = gameStatus;
        this.ship = ship;
    }

    public void Initialize()
    {
        disposables.Add(gameStatus.WhenPlayerShot
                .Subscribe(_ => camera.Shake(0.75f, TimeSpan.FromSeconds(0.5))));
        
        disposables.Add(ship.WhenCardBoarded
            .Where(type => (type & CardType.Monster) != 0)
            .Subscribe(_ => camera.Shake(0.25f, TimeSpan.FromSeconds(1), 4)));
        
        disposables.Add(gameStatus.WhenPlayerAttackedOnBoard
            .Merge(gameStatus.WhenPlayerConfronted)
            .Subscribe(_ => camera.Shake(0.15f, TimeSpan.FromSeconds(0.5), 2)));
    }

    public void Dispose()
    {
        disposables?.Dispose();
    }
}