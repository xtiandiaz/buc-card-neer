using System;
using UniRx;
using Zenject;

public interface IGameAudioController : IInitializable, IDisposable
{
}

public class GameAudioController : IGameAudioController
{
    private readonly IAudioManager audioManager;
    private readonly IRoutingController router;
    private readonly IDismissalController dismisser;
    private readonly IDefermentController deferrer;
    private readonly IForwardingController forwarder;
    private readonly IShootingController shooter;
    private readonly IMatchingController matcher;
    private readonly IGameStatus gameStatus;
    private readonly IBoard board;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private GameAudioController(
        IAudioManager audioManager,
        IRoutingController router,
        IDismissalController dismisser,
        IDefermentController deferrer,
        IForwardingController forwarder,
        IShootingController shooter,
        IMatchingController matcher,
        IGameStatus gameStatus,
        IBoard board
    )
    {
        this.audioManager = audioManager;
        this.router = router;
        this.dismisser = dismisser;
        this.deferrer = deferrer;
        this.forwarder = forwarder;
        this.shooter = shooter;
        this.matcher = matcher;
        this.gameStatus = gameStatus;
        this.board = board;
    }
    
    public void Initialize()
    {
        #region Player

        disposables.Add(gameStatus.WhenPlayerLost
            .Subscribe(_ => Play(AudioEventKey.CardAvatarDeath)));
        
        disposables.Add(gameStatus.WhenPlayerWon
            .Subscribe(_ => Play(AudioEventKey.GameWin)));

        #endregion
        
        #region Card Handling

        disposables.Add(router.WhenCardDropped
            .Subscribe(_ => Play(AudioEventKey.UIDragCancel)));
        
        disposables.Add(router.WhenCardPicked
            .Subscribe(_ => Play(AudioEventKey.UIDragGrab)));
        
        disposables.Add(dismisser.WhenCardDismissed
            .Subscribe(_ => Play(AudioEventKey.CardBridgeDismiss)));
        
        #endregion

        #region Card Matching

        disposables.Add(matcher.WhenMatchedDevice
            .Merge(deferrer.WhenMatchedDevice)
            .Subscribe(deviceType =>
            {
                switch (deviceType)
                {
                    case ArtificeType.Catapult:
                        Play(AudioEventKey.CardToolRangedUseCatapult);
                        break;
                    case ArtificeType.MidasTouch:
                        Play(AudioEventKey.CardItemTradeSell);
                        break;
                    case ArtificeType.TraderSpell:
                        Play(AudioEventKey.CardItemTradeBuy);
                        break;
                }
            }));

        disposables.Add(matcher.WhenDeviceActed
            .Merge(deferrer.WhenDeviceActed)
            .Subscribe(deviceType =>
            {
                switch (deviceType)
                {
                    case ArtificeType.Catapult:
                        Play(AudioEventKey.CardToolRangedHitCatapult);
                        break;
                }
            }));

        #endregion

        #region Ship

        disposables.Add(board.Ship.WhenCardBoarded
            .Subscribe(cardType => audioManager.Play(AudioEventSwitchKey.CardBoard, cardType)));
        
        disposables.Add(board.Ship.WhenCardRevealed
            .Merge(forwarder.WhenCardRevealed)
            .Subscribe(cardType => audioManager.Play(AudioEventSwitchKey.CardReveal, cardType)));
        
        disposables.Add(board.Ship.WhenCardStashed
            .Merge(forwarder.WhenCardStashed)
            .Subscribe(cardType => audioManager.Play(AudioEventSwitchKey.CardStash, cardType)));
        
        #endregion

        #region Sea

        disposables.Add(board.Sea.WhenArranged
            .Subscribe(_ => Play(AudioEventKey.CardSupplyCascade)));
        
        disposables.Add(board.Sea.WhenResupplied
            .Merge(deferrer.WhenResupplied)
            .Subscribe(_ => Play(AudioEventKey.CardSupplyRedeal)));

        #endregion

        #region Ranged Combat

        disposables.Add(board.Ship.WhenArmed
            .Subscribe(_ => audioManager.Play(AudioEventKey.CardToolRangedArm)));
        
        disposables.Add(shooter.WhenShot
            .Subscribe(_ => audioManager.Play(AudioEventKey.CardToolRangedUseCannon)));
        
        disposables.Add(shooter.WhenHit
            .Subscribe(_ => audioManager.Play(AudioEventKey.CardToolRangedHitCannon)));

        #endregion
    }

    public void Dispose()
    {
        disposables.Dispose();
    }

    private void Play(AudioEventKey withKey)
    {
        audioManager.Play(withKey);
    }
}