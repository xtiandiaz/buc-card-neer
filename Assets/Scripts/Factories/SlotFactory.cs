using System;

public class SlotFactory : ISlotFactory
{
    private readonly SlotBoarding.Factory modelBoardingFactory;
    private readonly SlotDefense.Factory modelDefenseFactory;
    private readonly SlotPlayer.Factory modelPlayerFactory;
    private readonly SlotResource.Factory modelResourceFactory;
    private readonly SlotEvent.Factory modelEventFactory;
    private readonly SlotController.Factory controllerFactory;

    private SlotFactory(
        SlotBoarding.Factory modelBoardingFactory,
        SlotDefense.Factory modelDefenseFactory,
        SlotPlayer.Factory modelPlayerFactory,
        SlotResource.Factory modelResourceFactory,
        SlotEvent.Factory modelEventFactory,
        SlotController.Factory controllerFactory
        )
    {
        this.modelBoardingFactory = modelBoardingFactory;
        this.modelDefenseFactory = modelDefenseFactory;
        this.modelPlayerFactory = modelPlayerFactory;
        this.modelResourceFactory = modelResourceFactory;
        this.modelEventFactory = modelEventFactory;
        this.controllerFactory = controllerFactory;
    }
    
    public ISlot Create(ISlotView forModel)
    {
        var model = CreateModel(forModel.Type, forModel.Capacity);
        var controller = controllerFactory.Create(model, forModel);
        
        model.Initialize();
        controller.Initialize();

        return model;
    }

    private ISlot CreateModel(SlotType forType, uint withCapacity)
    {
        switch (forType)
        {
            case SlotType.Event:
                return modelEventFactory.Create(withCapacity);
            case SlotType.Boarding:
                return modelBoardingFactory.Create(withCapacity);
            case SlotType.Defense:
                return modelDefenseFactory.Create(withCapacity);
            case SlotType.Resource:
                return modelResourceFactory.Create(withCapacity);
            case SlotType.Player:
                return modelPlayerFactory.Create(withCapacity);
            case SlotType.All:
            default:
                throw new ArgumentOutOfRangeException(nameof(forType), forType, null);
        }
    }
}