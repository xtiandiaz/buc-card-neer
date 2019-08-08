using System;
using UniRx;
using Zenject;

public interface ISupplyController : IInitializable, IDisposable
{
    IObservable<Unit> WhenRoundCompleted { get; }

    IObservable<Unit> Supply();
}

public class SupplyController : ISupplyController
{
    private readonly IClashingController clashingController;
    private readonly ISea sea;
    private readonly IAudioManager audioManager;
    private readonly IGameStatus gameStatus;
    private readonly Subject<Unit> roundCompletion = new Subject<Unit>();
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private SupplyController(
        IClashingController clashingController,
        ISea sea,
        IAudioManager audioManager,
        IGameStatus gameStatus
        )
    {
        this.clashingController = clashingController;
        this.sea = sea;
        this.audioManager = audioManager;
        this.gameStatus = gameStatus;
    }

    public IObservable<Unit> WhenRoundCompleted => roundCompletion;

    public void Initialize()
    {
        disposables.Add(clashingController.WhenRoundCompleted
            .SelectMany(_ => sea.Arrange()
                .DoOnSubscribe(() =>
                {
                    if (sea.ShouldArrange)
                        audioManager.Play(AudioEventKey.CardSupplyCascade);
                })
                .Concat(sea.Resupply()
                    .DoOnSubscribe(() =>
                    {
                        if (sea.ShouldResupply)
                            audioManager.Play(AudioEventKey.CardSupplyRedeal);
                    }))
                .DoOnCompleted(() => roundCompletion.OnNext(Unit.Default)))
            .Subscribe());
    }

    public IObservable<Unit> Supply()
    {
        return sea.Supply()
            .DoOnCompleted(() => gameStatus.DidSupplyOnce = true);
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}