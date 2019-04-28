using System.Linq;
using Zenject;

public class OceanFactory : IFactory<IOceanView, IOcean>
{
    private readonly Ocean.Factory modelFactory;
    private readonly OceanController.Factory controllerFactory;
    private readonly SlotFactory slotFactory;

    private OceanFactory(
        Ocean.Factory modelFactory, 
        OceanController.Factory controllerFactory,
        SlotFactory slotFactory
        )
    {
        this.modelFactory = modelFactory;
        this.controllerFactory = controllerFactory;
        this.slotFactory = slotFactory;
    }

    public IOcean Create(IOceanView withView)
    {
        var model = modelFactory.Create(withView.Slots.Select(sv => slotFactory.Create(sv)).ToArray());
        var controller = controllerFactory.Create(model, withView);

        return model;
    }
}