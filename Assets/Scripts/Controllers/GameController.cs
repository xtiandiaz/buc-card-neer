using System;
using UniRx;
using Zenject;

public interface IGameController : IInitializable, IDisposable
{
}

public class GameController : IGameController
{
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    
    private readonly IGameStatus status;
    private readonly ISupplyController supplyController;
    private readonly IClashingController clashingController;
    private readonly ICardHost cardHost;
    private readonly IShip ship;
    private readonly IPlayerCard player;
    private readonly IAudioManager audioManager;
    private readonly IGameCamera camera;
    private readonly ISea sea;

    public GameController(
        IGameStatus status,
        ISupplyController supplyController,
        IClashingController clashingController,
        ICardHost cardHost,
        IShip ship,
        IPlayerCard player,
        IAudioManager audioManager,
        IGameCamera camera
    )
    {
        this.status = status;
        this.supplyController = supplyController;
        this.clashingController = clashingController;
        this.cardHost = cardHost;
        this.ship = ship;
        this.player = player;
        this.audioManager = audioManager;
        this.camera = camera;
    }

    public void Initialize()
    {
        disposables.Add(cardHost.Lodge(player, ship.Helm)
            .SelectMany(_ => supplyController.Supply()
                .DoOnSubscribe(() => audioManager.Play(AudioEventKey.GameAssemble)))
            .DelaySubscription(TimeSpan.FromSeconds(0.5f))
            .Subscribe());
        
        disposables.Add(status.WhenLost
            .Subscribe(_ =>
            {
                OnGameEnded();
                audioManager.Play(AudioEventKey.CardAvatarDeath);
            }));
        
        disposables.Add(status.WhenWon
            .Subscribe(_ => OnGameEnded()));
        
        disposables.Add(
            status.WhenPlayerShot
                .Subscribe(_ => camera.Shake(0.75f, TimeSpan.FromSeconds(0.5))));
        
        disposables.Add(status.WhenPlayerBoardedCard
            .Where(type => (type & CardType.Monster) != 0)
            .Subscribe(_ => camera.Shake(0.25f, TimeSpan.FromSeconds(1), 4)));
        
        disposables.Add(status.WhenPlayerAttackedOnBoard
            .Merge(status.WhenPlayerConfronted)
            .Subscribe(_ => camera.Shake(0.15f, TimeSpan.FromSeconds(0.5), 2)));
    }

    public void Dispose()
    {
        disposables.Dispose();
    }

    private void OnGameEnded()
    {
        clashingController.Dispose();
        supplyController.Dispose();
    }
}