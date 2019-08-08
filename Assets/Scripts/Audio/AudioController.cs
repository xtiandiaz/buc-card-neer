using System;
using UniRx;
using Zenject;

public class AudioController : IInitializable, IDisposable
{
    private readonly IAudioManager audioManager;
    private readonly ICardRouter cardRouter;
    private readonly ICardDismisser cardDismisser;
    private readonly IBoardingController boardingController;
    private readonly ICardForwarder cardForwarder;
    private readonly IGameStatus gameStatus;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private AudioController(
        IAudioManager audioManager,
        ICardRouter cardRouter,
        ICardDismisser cardDismisser,
        IBoardingController boardingController,
        ICardForwarder cardForwarder,
        IGameStatus gameStatus
        )
    {
        this.audioManager = audioManager;
        this.cardRouter = cardRouter;
        this.cardDismisser = cardDismisser;
        this.boardingController = boardingController;
        this.cardForwarder = cardForwarder;
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
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}