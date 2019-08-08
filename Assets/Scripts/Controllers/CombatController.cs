using System;
using UniRx;
using Zenject;

public interface ICombatController : IInitializable, IDisposable
{
}

public class CombatController : ICombatController
{
    private readonly ICardShooter shooter;
    private readonly IShip ship;
    private readonly ISea sea;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private CombatController(
        ICardShooter shooter,
        IShip ship,
        ISea sea
    )
    {
        this.shooter = shooter;
        this.ship = ship;
        this.sea = sea;
    }
    
    public void Initialize()
    {
        disposables.Add(ship.WhenArmed
            .Delay(TimeSpan.FromSeconds(0.5))
            .SelectMany(_ => shooter.Shoot(ship.Plank, sea.Slots))
            .Do(_ => ship.Unlock())
            .Subscribe());
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}