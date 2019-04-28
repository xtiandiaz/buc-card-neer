using System;
using UnityEngine;
using Zenject;

public interface IShipController
{
}

public class ShipController : IShipController, IDisposable
{
    public class Factory : PlaceholderFactory<IShip, IShipView, ShipController>
    {
    }
    
    private readonly IShip model;
    private readonly IShipView view;

    private ShipController(IShip model, IShipView view)
    {
        this.model = model;
        this.view = view;
    }

    public void Dispose()
    {
    }
}