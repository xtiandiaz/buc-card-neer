using System;
using UniRx;
using UnityEngine;
using Zenject;

public interface IShipController
{
    void Initialize();
}

public class ShipController : IShipController, IDisposable
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

    public virtual void Initialize()
    {
        disposables.Add(model.Docking.Subscribe(position => view.Dock(position)));
        disposables.Add(model.Sailing.Subscribe(position => view.SetSail(position)));
    }

    public void Dispose()
    {
        disposables?.Dispose();
    }
}