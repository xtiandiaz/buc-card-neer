using Zenject;

public class CardSlotFactory : IFactory<ICardSlotView, CardSlotController>
{
    private readonly CardSlot.Factory cardSlotFactory;
    private readonly CardSlotView.Factory cardSlotViewFactory;
    private readonly CardSlotController.Factory cardSlotControllerFactory;

    private CardSlotFactory(
        CardSlot.Factory cardSlotFactory, 
        CardSlotView.Factory cardSlotViewFactory,
        CardSlotController.Factory cardSlotControllerFactory
        )
    {
        this.cardSlotFactory = cardSlotFactory;
        this.cardSlotViewFactory = cardSlotViewFactory;
        this.cardSlotControllerFactory = cardSlotControllerFactory;
    }

    public CardSlotController Create(ICardSlotView fromView)
    {
        return cardSlotControllerFactory.Create(cardSlotFactory.Create(fromView.InitialCapacity), fromView);
    }
}