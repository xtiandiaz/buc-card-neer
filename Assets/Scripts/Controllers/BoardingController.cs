using System;
using UniRx;
using Zenject;

public interface IBoardingController : IInitializable, IDisposable
{
    IObservable<Unit> WhenBoarded { get; }
}

public class BoardingController : IBoardingController
{
    private readonly ICardHost cardHost;
    private readonly Subject<Unit> boarding = new Subject<Unit>();
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    
    private readonly IShip ship;

    private BoardingController(
        ICardHost cardHost,
        IShip ship
        )
    {
        this.cardHost = cardHost;
        this.ship = ship;
    }

    public IObservable<Unit> WhenBoarded => boarding;

    public void Initialize()
    {
        disposables.Add(ship.Plank.WhenLodged
            .Where(card => !card.IsBoarded)
            .Do(card => card.IsBoarded = true)
            .SelectMany(card => card.IsLocked ? Observable.ReturnUnit() : Handle(card))
            .Do(boarding.OnNext)
            .Subscribe());

        disposables.Add(ship.Plank.WhenLodged
            .Where(card => card.IsLocked)
            .SelectMany(card => card.WhenUnlocked
                .ContinueWith(_ => card.Drop())
                .Delay(TimeSpan.FromSeconds(0.15))
                .ContinueWith(_ => Handle(card))
                .Do(boarding.OnNext))
            .Subscribe());
    }

    public void Dispose()
    {
        boarding.Dispose();
        disposables.Dispose();
    }

    private IObservable<Unit> Handle(ICard card)
    {
        return card.Reveal()
            .ContinueWith(_ => card.IsResource
                ? cardHost.Lodge(ship.Plank, ship.GetStash(card.Type))
                : Observable.Empty<Unit>())
            .LastOrDefault();
    }
}