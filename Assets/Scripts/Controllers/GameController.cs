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
    private readonly ILodgingController lodger;
    private readonly IDealingController dealer;
    private readonly IShip ship;
    private readonly IPlayerCard player;
    private readonly IAudioManager audioManager;
    private readonly IPlayerSettings playerSettings;
    private readonly IMenuFactory menuFactory;
    private readonly IFloatingBannerFactory bannerFactory;
    private readonly ISea sea;

    private readonly DeviceType[] devices = {DeviceType.Catapult, DeviceType.MidasTouch, DeviceType.TraderSpell};

    public GameController(
        IGameStatus status,
        IBoardController boardController,
        ILodgingController lodger,
        IDealingController dealer,
        IShip ship,
        ISea sea,
        IPlayerCard player,
        IAudioManager audioManager,
        IPlayerSettings playerSettings,
        IMenuFactory menuFactory
    )
    {
        this.status = status;
        this.boardController = boardController;
        this.lodger = lodger;
        this.dealer = dealer;
        this.ship = ship;
        this.sea = sea;
        this.player = player;
        this.audioManager = audioManager;
        this.playerSettings = playerSettings;
        this.menuFactory = menuFactory;

        devices.Shuffle();
    }

    public void Initialize()
    {
        disposables.Add(ship.Helm.Lodge(player)
            .ContinueWith(_ => playerSettings.ShouldDealDeviceCards 
                ? dealer.Deal(devices, ship.Mount, 0.1)
                : Observable.ReturnUnit())
            .ContinueWith(_ => sea.Supply()
                .DoOnSubscribe(() => audioManager.Play(AudioEventKey.GameAssemble))
                .DoOnCompleted(() => status.DidSupplyOnce = true))
            .DelaySubscription(TimeSpan.FromSeconds(0.5f))
            .Subscribe());
        
        disposables.Add(status.WhenLost
            .Subscribe(_ =>
            {
                OnGameEnded();
                menuFactory.Create<IGameOverMenu>();
            }));

        disposables.Add(status.WhenWon
            .Do(_ => OnGameEnded())
            .SelectMany(score => menuFactory.Create<IStageFinishedMenu>()
                .Feed(score))
            .Subscribe());
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