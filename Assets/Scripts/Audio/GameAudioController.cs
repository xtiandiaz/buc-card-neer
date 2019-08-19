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
    private readonly IShip ship;
    private readonly ISea sea;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private GameAudioController(
        IAudioManager audioManager,
        IRoutingController router,
        IDismissalController dismisser,
        IDefermentController deferrer,
        IForwardingController forwarder,
        IShootingController shooter,
        IMatchingController matcher,
        IShip ship,
        ISea sea
    )
    {
        this.audioManager = audioManager;
        this.router = router;
        this.dismisser = dismisser;
        this.deferrer = deferrer;
        this.forwarder = forwarder;
        this.shooter = shooter;
        this.matcher = matcher;
        this.ship = ship;
        this.sea = sea;
    }
    
    public void Initialize()
    {
        #region Card Handling

        disposables.Add(router.WhenCardDropped
            .Subscribe(_ => audioManager.Play(AudioEventKey.UIDragCancel)));
        
        disposables.Add(router.WhenCardPicked
            .Subscribe(_ => audioManager.Play(AudioEventKey.UIDragGrab)));
        
        disposables.Add(dismisser.WhenCardDismissed
            .Subscribe(_ => audioManager.Play(AudioEventKey.CardBridgeDismiss)));
        
        #endregion

        #region Card Matching

        disposables.Add(matcher.WhenMatchedDevice
            .Merge(deferrer.WhenMatchedDevice)
            .Subscribe(deviceType =>
            {
                switch (deviceType)
                {
                    case DeviceType.Catapult:
                        audioManager.Play(AudioEventKey.CardToolRangedUseCatapult);
                        break;
                    case DeviceType.MidasTouch:
                        audioManager.Play(AudioEventKey.CardItemTradeSell);
                        break;
                    case DeviceType.TraderSpell:
                        audioManager.Play(AudioEventKey.CardItemTradeBuy);
                        break;
                }
            }));

        disposables.Add(matcher.WhenDeviceActed
            .Merge(deferrer.WhenDeviceActed)
            .Subscribe(deviceType =>
            {
                switch (deviceType)
                {
                    case DeviceType.Catapult:
                        audioManager.Play(AudioEventKey.CardToolRangedHitCatapult);
                        break;
                }
            }));

        #endregion

        #region Ship

        disposables.Add(ship.WhenCardBoarded
            .Subscribe(cardType => audioManager.Play(AudioEventSwitchKey.CardBoard, cardType)));
        
        disposables.Add(ship.WhenCardRevealed
            .Merge(forwarder.WhenCardRevealed)
            .Subscribe(cardType => audioManager.Play(AudioEventSwitchKey.CardReveal, cardType)));
        
        disposables.Add(ship.WhenCardStashed
            .Merge(forwarder.WhenCardStashed)
            .Subscribe(cardType => audioManager.Play(AudioEventSwitchKey.CardStash, cardType)));
        
        #endregion

        #region Sea

        disposables.Add(sea.WhenArranged
            .Subscribe(_ => audioManager.Play(AudioEventKey.CardSupplyCascade)));
        
        disposables.Add(sea.WhenResupplied
            .Subscribe(_ => audioManager.Play(AudioEventKey.CardSupplyRedeal)));

        #endregion

        #region Ranged Combat

        disposables.Add(ship.WhenArmed
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
}