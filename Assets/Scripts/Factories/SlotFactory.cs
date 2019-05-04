using System;

public class SlotFactory : ISlotFactory
{
    private readonly SlotBoarding.Factory modelBoardingFactory;
    private readonly SlotDefense.Factory modelDefenseFactory;
    private readonly SlotPlayer.Factory modelPlayerFactory;
    private readonly SlotStorage.Factory modelResourceFactory;
    private readonly SlotEvent.Factory modelEventFactory;
    private readonly SlotController.Factory controllerFactory;

    private SlotFactory(
        SlotBoarding.Factory modelBoardingFactory,
        SlotDefense.Factory modelDefenseFactory,
        SlotPlayer.Factory modelPlayerFactory,
        SlotStorage.Factory modelResourceFactory,
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
    
    public ISlot Create(ISlotView fromView)
    {
        var model = CreateModel(fromView);
        var controller = controllerFactory.Create(model, fromView);
        
        controller.Initialize();

        return model;
    }

    private ISlot CreateModel(ISlotView fromView)
    {
        var type = fromView.Type;
        var capacity = fromView.Capacity;
        
        switch (type)
        {
            case SlotType.Event:
                return modelEventFactory.Create(capacity);
            case SlotType.Boarding:
                return modelBoardingFactory.Create(capacity);
            case SlotType.Defense:
                return modelDefenseFactory.Create(capacity);
            case SlotType.Storage:
                return modelResourceFactory.Create(fromView.ResourceMask, capacity);
            case SlotType.Player:
                return modelPlayerFactory.Create(capacity);
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}