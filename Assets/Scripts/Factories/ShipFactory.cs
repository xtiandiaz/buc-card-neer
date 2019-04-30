using System;
using System.Linq;

public class ShipFactory : IShipFactory
{
    private readonly ShipPlayer.Factory shipPlayerFactory;
    private readonly ShipMerchant.Factory shipMerchantFactory;
    private readonly ShipPirate.Factory shipPirateFactory;
    private readonly ShipController.Factory controllerFactory;
    private readonly ISlotFactory slotFactory;

    private ShipFactory(
        ShipPlayer.Factory shipPlayerFactory,
        ShipMerchant.Factory shipMerchantFactory,
        ShipPirate.Factory shipPirateFactory,
        ShipController.Factory controllerFactory,
        ISlotFactory slotFactory
    )
    {
        this.shipPlayerFactory = shipPlayerFactory;
        this.shipMerchantFactory = shipMerchantFactory;
        this.shipPirateFactory = shipPirateFactory;
        this.controllerFactory = controllerFactory;
        this.slotFactory = slotFactory;
    }
    
    public IShip Create(IShipView forModel)
    {
        var model = CreateModel(forModel);
        var controller = controllerFactory.Create(model, forModel);
        
        controller.Initialize();

        return model;
    }

    private IShip CreateModel(IShipView fromView)
    {
        var slots = fromView.Slots.Select(sv => slotFactory.Create(sv)).ToArray();
        
        switch (fromView.Type)
        {
            case ShipType.Player:

                return shipPlayerFactory.Create(slots);

            case ShipType.Pirate:

                return shipPirateFactory.Create(slots);
                
            case ShipType.Merchant:

                return shipMerchantFactory.Create(slots);
                
            default:
                throw new ArgumentOutOfRangeException(nameof(fromView.Type), fromView.Type, null);
        }
    }
}