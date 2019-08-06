using System;
using UniRx;
using Zenject;

public interface ISupplyController : IInitializable, IDisposable
{
    IObservable<Unit> Supply();
}

public class SupplyController : ISupplyController
{
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private readonly IClashingController clashingController;
    private readonly ISea sea;
    private readonly IAudioManager audioManager;
    private readonly IGameStatus gameStatus;

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

    public void Initialize()
    {
        disposables.Add(clashingController.WhenSeaClashed
            .SelectMany(_ => sea.Arrange()
                .DoOnSubscribe(() =>
                {
                    if (sea.IsMessy && !sea.ShouldResupply)
                        audioManager.Play(AudioEventKey.CardSupplyCascade);
                })
                .ContinueWith(__ =>
                {
                    if (sea.ShouldResupply)
                        audioManager.Play(AudioEventKey.CardSupplyRedeal);
                    
                    return sea.Resupply();
                }))
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