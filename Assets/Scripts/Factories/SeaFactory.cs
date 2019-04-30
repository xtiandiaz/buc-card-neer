using System.Linq;

public class SeaFactory : ISeaFactory
{
    private readonly Sea.Factory modelFactory;
    private readonly SeaController.Factory controllerFactory;
    private readonly ISlotFactory slotFactory;

    private SeaFactory(
        Sea.Factory modelFactory, 
        SeaController.Factory controllerFactory,
        ISlotFactory slotFactory
        )
    {
        this.modelFactory = modelFactory;
        this.controllerFactory = controllerFactory;
        this.slotFactory = slotFactory;
    }

    public ISea Create(ISeaView forModel)
    {
        var model = modelFactory.Create(forModel.Slots.Select(sv => slotFactory.Create(sv)).ToArray());
        var controller = controllerFactory.Create(model, forModel);
        
        controller.Initialize();

        return model;
    }
}