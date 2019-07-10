using System;
using UniRx;
using UnityEngine;
using Zenject;

public interface ISupplyController : IInitializable, IDisposable
{
    IObservable<Unit> Supply();
}

public class SupplyController : ISupplyController
{
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private readonly IClashingController clashingController;
    private readonly ICardDealer dealer;
    private readonly ISea sea;
    private readonly IAudioManager audioManager;

    private SupplyController(
        IClashingController clashingController,
        ICardDealer dealer, 
        ISea sea,
        IAudioManager audioManager
        )
    {
        this.clashingController = clashingController;
        this.dealer = dealer;
        this.sea = sea;
        this.audioManager = audioManager;
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
        return sea.Supply();
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}