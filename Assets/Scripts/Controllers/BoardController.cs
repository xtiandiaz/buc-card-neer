using System;
using UniRx;
using Zenject;

public interface IBoardController : IInitializable, IDisposable
{
}

public class BoardController : IBoardController
{
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    
    private readonly ISea sea;
    private readonly IShip ship;
    private readonly ICardShooter cardShooter;

    private BoardController(
        ISea sea,
        IShip ship,
        ICardShooter cardShooter
    )
    {
        this.sea = sea;
        this.ship = ship;
        this.cardShooter = cardShooter;
    }

    public void Initialize()
    {
        disposables.Add(ship.WhenCardBoarded.Take(1)
            .Do(_ => sea.Lock())
            .ContinueWith(ship.WhenCardHandled.Take(1))
            .ContinueWith(sea.Clash())
            .ContinueWith(_ => sea.Arrange())
            .ContinueWith(sea.Resupply())
            .DoOnCompleted(sea.Unlock)
            .RepeatSafe()
            .Subscribe());
        
        disposables.Add(ship.WhenArmed
            .Delay(TimeSpan.FromSeconds(0.5))
            .SelectMany(_ => cardShooter.Shoot(ship.Plank, sea.Slots))
            .Do(_ => ship.Unlock())
            .Subscribe());
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}