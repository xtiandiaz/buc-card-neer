using System;
using UniRx;
using Zenject;

public interface IGameStatus
{
    IObservable<Unit> WhenLost { get; }
}

public interface IGameController : IGameStatus, IInitializable, IDisposable
{
    void Reset();
}

public class GameController : IGameController
{
    private readonly Subject<Unit> losing = new Subject<Unit>();
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    
    private readonly IAppController appController;
    private readonly ISupplyController supplyController;
    private readonly IClashingController clashingController;
    private readonly ICardHost cardHost;
    private readonly IDeck deck;
    private readonly IShip ship;
    private readonly IPlayerCard player;
    private readonly ISea sea;

    public GameController(
        IAppController appController,
        ISupplyController supplyController,
        IClashingController clashingController,
        ICardHost cardHost,
        IShip ship,
        IPlayerCard player
        )
    {
        this.appController = appController;
        this.supplyController = supplyController;
        this.clashingController = clashingController;
        this.cardHost = cardHost;
        this.ship = ship;
        this.player = player;
    }

    public IObservable<Unit> WhenLost => losing;
    
    public void Initialize()
    {
        disposables.Add(supplyController.Supply()
            .SelectMany(_ => cardHost.Lodge(player, ship.Helm))
            .Subscribe());
        
        disposables.Add(player.WhenDestroyed
            .Merge(player.WhenBankrupt)
            .Take(1)
            .Subscribe(_ =>
            {
                clashingController.Dispose();
                supplyController.Dispose();
                
                losing.OnNext(Unit.Default);
            }));
    }

    public void Reset()
    {
        appController.Reload();
    }

    public void Dispose()
    {
        losing.Dispose();
        disposables.Dispose();
    }
}