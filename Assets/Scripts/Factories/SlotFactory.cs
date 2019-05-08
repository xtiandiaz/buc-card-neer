using System;

public class SlotFactory : ISlotFactory
{
    private readonly Pile.Factory pileFactory;
    private readonly SlotBoarding.Factory modelBoardingFactory;
    private readonly SlotPlayer.Factory modelPlayerFactory;
    private readonly SlotStorage.Factory modelResourceFactory;
    private readonly SlotEvent.Factory modelEventFactory;
    private readonly SlotController.Factory controllerFactory;

    private SlotFactory(
        Pile.Factory pileFactory,
        SlotBoarding.Factory modelBoardingFactory,
        SlotPlayer.Factory modelPlayerFactory,
        SlotStorage.Factory modelResourceFactory,
        SlotEvent.Factory modelEventFactory,
        SlotController.Factory controllerFactory
        )
    {
        this.pileFactory = pileFactory;
        this.modelBoardingFactory = modelBoardingFactory;
        this.modelPlayerFactory = modelPlayerFactory;
        this.modelResourceFactory = modelResourceFactory;
        this.modelEventFactory = modelEventFactory;
        this.controllerFactory = controllerFactory;
    }
    
    public ISlot Create(ISlotView fromView)
    {
        var model = CreateModel(fromView);
       
        controllerFactory.Create(model, fromView);

        return model;
    }

    private ISlot CreateModel(ISlotView fromView)
    {
        var type = fromView.Type;
        var pileExtent = fromView.Capacity > 0 ? (int) fromView.Capacity : default(int?);
        var pile = pileFactory.Create(fromView.CardArrangement, pileExtent);
        
        switch (type)
        {
            case SlotType.Event:
                return modelEventFactory.Create(pile);
            case SlotType.Boarding:
                return modelBoardingFactory.Create(pile);
            case SlotType.Storage:
                return modelResourceFactory.Create(fromView.ResourceMask, pile);
            case SlotType.Player:
                return modelPlayerFactory.Create(pile);
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}