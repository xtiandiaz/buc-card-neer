using System;
using UniRx;
using Zenject;

public interface IGameStatus
{
    IObservable<Unit> WhenLost { get; }
    IObservable<int> UndealtCardCount { get; }

    void Reset();
}

public interface IGameController : IGameStatus, IInitializable, IDisposable
{
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
    private readonly IAudioManager audioManager;
    private readonly ISea sea;

    public GameController(
        IAppController appController,
        ISupplyController supplyController,
        IClashingController clashingController,
        ICardHost cardHost,
        IShip ship,
        IPlayerCard player,
        IAudioManager audioManager,
        IDeck deck
        )
    {
        this.appController = appController;
        this.supplyController = supplyController;
        this.clashingController = clashingController;
        this.cardHost = cardHost;
        this.ship = ship;
        this.player = player;
        this.audioManager = audioManager;
        this.deck = deck;
    }

    public IObservable<Unit> WhenLost => losing;
    public IObservable<int> UndealtCardCount => deck.CardCount;
    
    public void Initialize()
    {
        disposables.Add(cardHost.Lodge(player, ship.Helm)
            .SelectMany(_ => supplyController.Supply()
                .DoOnSubscribe(() => audioManager.Play(AudioEventKey.GameAssemble)))
            .DelaySubscription(TimeSpan.FromSeconds(0.5f))
            .Subscribe());
        
        disposables.Add(player.WhenDestroyed
            .Do(_ => audioManager.Play(AudioEventKey.CardAvatarDeath))
            .Merge(player.WhenBankrupt)
            .Take(1)
            .Delay(TimeSpan.FromSeconds(0.5))
            .Subscribe(_ =>
            {
                clashingController.Dispose();
                supplyController.Dispose();
                
                audioManager.Play(AudioEventKey.GameLose);
                
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