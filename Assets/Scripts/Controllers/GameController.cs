using System;
using UniRx;
using Zenject;

public interface IGameController : IInitializable, IDisposable
{
}

public class GameController : IGameController
{
    private readonly Subject<Unit> losing = new Subject<Unit>();
    private readonly Subject<int> winning = new Subject<int>();
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    
    private readonly ArtificeType[] artifices = {ArtificeType.Catapult, ArtificeType.MidasTouch, ArtificeType.TraderSpell};
    
    private readonly IGameStatus gameStatus;
    private readonly ILodgingController lodger;
    private readonly IDealingController dealer;
    private readonly IShootingController shooter;
    private readonly IPlayerCard player;
    private readonly IAudioManager audioManager;
    private readonly IPlayerSettings playerSettings;
    private readonly IMenuFactory menuFactory;
    private readonly IBoard board;

    public GameController(
        IAppStatus appStatus,
        IGameStatus gameStatus,
        IStage stage,
        IBoard board,
        ILodgingController lodger,
        IDealingController dealer,
        IShootingController shooter,
        IPlayerCard player,
        IAudioManager audioManager,
        IPlayerSettings playerSettings,
        IMenuFactory menuFactory
    )
    {
        this.gameStatus = gameStatus;
        this.board = board;
        this.lodger = lodger;
        this.dealer = dealer;
        this.shooter = shooter;
        this.player = player;
        this.audioManager = audioManager;
        this.playerSettings = playerSettings;
        this.menuFactory = menuFactory;

        artifices.Shuffle();
    }

    public void Initialize()
    {        
        disposables.Add(board.Ship.Helm.Lodge(player)
            .ContinueWith(_ => playerSettings.ShouldDealDeviceCards 
                ? dealer.Deal(artifices, board.Ship.Mount, 0.1)
                : Observable.ReturnUnit())
            .ContinueWith(_ => board.Sea.Supply()
                .DoOnSubscribe(() => audioManager.Play(AudioEventKey.GameAssemble))
                .DoOnCompleted(() => gameStatus.DidSupplyOnce = true))
            .DelaySubscription(TimeSpan.FromSeconds(0.5f))
            .Subscribe());
        
        disposables.Add(gameStatus.WhenPlayerLost
            .DoOnCompleted(() => Dispose())
            .Subscribe(_ => menuFactory.Create<IGameOverMenu>()));

        disposables.Add(gameStatus.WhenPlayerWon
            .SelectMany(score => menuFactory.Create<IStageFinishedMenu>()
                .Feed(score))
            .DoOnCompleted(() => Dispose())
            .Subscribe());

        disposables.Add(board.Ship.WhenCardBoarded.Take(1)
            .Do(_ => board.Sea.Lock())
            .ContinueWith(board.Ship.WhenCardHandled.Take(1))
            .ContinueWith(board.Sea.Clash())
            .ContinueWith(_ => board.Sea.Arrange())
            .ContinueWith(board.Sea.Resupply())
            .DoOnCompleted(board.Sea.Unlock)
            .RepeatSafe()
            .Subscribe());
        
        disposables.Add(board.Ship.WhenCardStashed
            .Subscribe(cardType =>
            {
                gameStatus.PlayerDidStashItem |= (cardType & CardType.Item) != 0;
                gameStatus.PlayerDidStashTool |= (cardType & CardType.Tool) != 0;
            }));
        
        disposables.Add(board.Ship.WhenArmed.Take(1)
            .Do(_ =>
            {
                board.Sea.Lock();
                board.Ship.Lock();
            })
            .Delay(TimeSpan.FromSeconds(0.5))
            .ContinueWith(_ => shooter.Shoot(board.Ship.Plank, board.Sea.Slots))
            .ContinueWith(_ => board.Sea.Arrange())
            .ContinueWith(board.Sea.Resupply())
            .DoOnCompleted(() =>
            {
                board.Sea.Unlock();
                board.Ship.Unlock();
            })
            .RepeatSafe()
            .Subscribe());
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}