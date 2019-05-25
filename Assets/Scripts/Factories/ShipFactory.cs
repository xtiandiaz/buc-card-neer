using System;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

public interface IShipFactory : IFactory<IShip>, IDisposable
{
}

public class ShipFactory : IShipFactory
{
    private readonly Ship.Factory modelFactory;
    private readonly ShipView.Factory viewFactory;
    private readonly ShipController.Factory controllerFactory;
    private readonly ISlotFactory slotFactory;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private ShipFactory(
        Ship.Factory modelFactory,
        ShipView.Factory viewFactory,
        ShipController.Factory controllerFactory,
        ISlotFactory slotFactory
        )
    {
        this.modelFactory = modelFactory;
        this.viewFactory = viewFactory;        
        this.controllerFactory = controllerFactory;
        this.slotFactory = slotFactory;
    }
    
    public IShip Create()
    {
        var view = viewFactory.Create();
        var slots = view.Slots.Select(slotFactory.Create).ToArray();
        var model = modelFactory.Create(slots);
        
        disposables.Add(controllerFactory.Create(model, view));

        return model;
    }
    
    public void Dispose()
    {
        disposables.Dispose();
    }
}