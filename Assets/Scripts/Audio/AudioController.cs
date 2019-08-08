using System;
using UniRx;
using Zenject;

public interface IAudioController : IInitializable, IDisposable
{
}

public class AudioController : IAudioController
{
    private readonly IAudioManager audioManager;
    private readonly ICardRouter cardRouter;
    private readonly ICardDismisser cardDismisser;
    private readonly IBoardingController boardingController;
    private readonly ICardForwarder cardForwarder;
    private readonly ICardShooter cardShooter;
    private readonly IShip ship;
    private readonly IGameStatus gameStatus;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private AudioController(
        IAudioManager audioManager,
        ICardRouter cardRouter,
        ICardDismisser cardDismisser,
        IBoardingController boardingController,
        ICardForwarder cardForwarder,
        ICardShooter cardShooter,
        IShip ship,
        IGameStatus gameStatus
        )
    {
        this.audioManager = audioManager;
        this.cardRouter = cardRouter;
        this.cardDismisser = cardDismisser;
        this.boardingController = boardingController;
        this.cardForwarder = cardForwarder;
        this.cardShooter = cardShooter;
        this.ship = ship;
        this.gameStatus = gameStatus;
    }
    
    public void Initialize()
    {
        disposables.Add(cardRouter.WhenCardDropped
            .Subscribe(_ => audioManager.Play(AudioEventKey.UIDragCancel)));
        
        disposables.Add(cardRouter.WhenCardPicked
            .Subscribe(_ => audioManager.Play(AudioEventKey.UIDragGrab)));
        
        disposables.Add(cardDismisser.WhenCardDismissed
            .Subscribe(_ => audioManager.Play(AudioEventKey.CardBridgeDismiss)));
        
        disposables.Add(gameStatus.WhenPlayerBoardedCard
            .Subscribe(cardType => audioManager.Play(AudioEventSwitchKey.CardBoard, cardType)));
        
        disposables.Add(boardingController.WhenCardRevealed
            .Merge(cardForwarder.WhenCardRevealed)
            .Subscribe(cardType => audioManager.Play(AudioEventSwitchKey.CardReveal, cardType)));
        
        disposables.Add(boardingController.WhenCardStashed
            .Merge(cardForwarder.WhenCardStashed)
            .Subscribe(cardType => audioManager.Play(AudioEventSwitchKey.CardStash, cardType)));

        #region Ranged Combat

        disposables.Add(ship.WhenArmed
            .Subscribe(_ => audioManager.Play(AudioEventKey.CardToolRangedArm)));
        
        disposables.Add(cardShooter.WhenShot
            .Subscribe(_ => audioManager.Play(AudioEventKey.CardToolRangedUseCannon)));
        
        disposables.Add(cardShooter.WhenHit
            .Subscribe(_ => audioManager.Play(AudioEventKey.CardToolRangedHitCannon)));

        #endregion
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}