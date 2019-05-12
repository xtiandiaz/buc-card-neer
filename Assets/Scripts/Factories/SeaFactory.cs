using System.Linq;
using UnityEngine;
using Zenject;

public interface ISeaFactory : IFactory<ISea>
{
}

public class SeaFactory : ISeaFactory
{
    private readonly Sea.Factory modelFactory;
    private readonly SeaView.Factory viewFactory;
    private readonly SeaController.Factory controllerFactory;
    private readonly ISlotFactory slotFactory;

    private SeaFactory(
        Sea.Factory modelFactory, 
        SeaView.Factory viewFactory, 
        SeaController.Factory controllerFactory,
        ISlotFactory slotFactory
        )
    {
        this.modelFactory = modelFactory;
        this.viewFactory = viewFactory;
        this.controllerFactory = controllerFactory;
        this.slotFactory = slotFactory;
    }

    public ISea Create()
    {
        var view = viewFactory.Create();
        var model = modelFactory.Create(view.Slots.Select(slotView => slotFactory.Create(slotView)).ToArray());
        
        controllerFactory.Create(model, view);

        return model;
    }
}