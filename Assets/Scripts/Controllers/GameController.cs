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
    private readonly IBoardController boardController;
    private readonly ICardHost cardHost;
    private readonly IShip ship;
    private readonly IPlayerCard player;
    private readonly IAudioManager audioManager;
    private readonly ISea sea;

    public GameController(
        IGameStatus status,
        IBoardController boardController,
        ICardHost cardHost,
        IShip ship,
        ISea sea,
        IPlayerCard player,
        IAudioManager audioManager
    )
    {
        this.status = status;
        this.boardController = boardController;
        this.cardHost = cardHost;
        this.ship = ship;
        this.sea = sea;
        this.player = player;
        this.audioManager = audioManager;
    }

    public void Initialize()
    {
        disposables.Add(cardHost.Lodge(player, ship.Helm)
            .SelectMany(_ => sea.Supply()
                .DoOnSubscribe(() => audioManager.Play(AudioEventKey.GameAssemble))
                .DoOnCompleted(() => status.DidSupplyOnce = true))
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
    }

    public void Dispose()
    {
        disposables.Dispose();
    }

    private void OnGameEnded()
    {
        boardController.Dispose();
    }
}