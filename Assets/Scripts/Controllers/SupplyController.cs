using System;
using UniRx;
using UnityEngine;
using Zenject;

public interface ISupplyController : IInitializable, IDisposable
{
    IObservable<Unit> WhenSuppliedFirstTime { get; }

    IObservable<Unit> Supply();
}

public class SupplyController : ISupplyController
{
    private readonly Subject<Unit> supply =  new Subject<Unit>();
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private readonly IClashingController clashingController;
    private readonly ISea sea;
    private readonly IAudioManager audioManager;

    private SupplyController(
        IClashingController clashingController,
        ISea sea,
        IAudioManager audioManager
        )
    {
        this.clashingController = clashingController;
        this.sea = sea;
        this.audioManager = audioManager;
    }

    public IObservable<Unit> WhenSuppliedFirstTime => supply.First();

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
            .DoOnCompleted(() => supply.OnNext(Unit.Default));
    }

    public void Dispose()
    {
        supply.Dispose();
        disposables.Dispose();
    }
}