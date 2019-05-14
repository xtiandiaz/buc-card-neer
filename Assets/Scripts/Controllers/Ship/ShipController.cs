using System;
using UniRx;
using UnityEngine;
using Zenject;

public interface IShipController : IInitializable, IDisposable
{
}

public class ShipController : IShipController
{
    public class Factory : PlaceholderFactory<IShip, IShipView, ShipController>
    {
    }
    
    protected readonly CompositeDisposable disposables = new CompositeDisposable();
    
    private readonly IShip model;
    private readonly IShipView view;

    protected ShipController(IShip model, IShipView view)
    {
        this.model = model;
        this.view = view;
    }

    [Inject]
    public virtual void Initialize()
    {
        disposables.Add(model.WhenDocked.Subscribe(_ => view.Dock(model.Position)));
        disposables.Add(model.WhenSailed.Subscribe(_ => view.SetSail(model.Position)));
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}