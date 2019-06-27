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

    private SupplyController(
        IClashingController clashingController,
        ICardDealer dealer, 
        ISea sea)
    {
        this.clashingController = clashingController;
        this.dealer = dealer;
        this.sea = sea;
    }

    public void Initialize()
    {
        disposables.Add(clashingController.WhenSeaClashed
            .SelectMany(_ => sea.Arrange().ContinueWith(__ => sea.Resupply()))
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