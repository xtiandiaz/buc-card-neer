using System;
using System.Linq;
using Zenject;

public class ShipFactory : IFactory<IShipView, IShip>
{
    private readonly ShipPlayer.Factory shipPlayerFactory;
    private readonly ShipController.Factory controllerFactory;
    private readonly SlotFactory slotFactory;

    private ShipFactory(
        ShipPlayer.Factory shipPlayerFactory,
        ShipController.Factory controllerFactory,
        SlotFactory slotFactory
    )
    {
        this.shipPlayerFactory = shipPlayerFactory;
        this.controllerFactory = controllerFactory;
        this.slotFactory = slotFactory;
    }
    
    public IShip Create(IShipView withView)
    {
        var model = CreateModel(withView);
        var controller = controllerFactory.Create(model, withView);

        return model;
    }

    private IShip CreateModel(IShipView fromView)
    {
        var slots = fromView.Slots.Select(sv => slotFactory.Create(sv));
        
        switch (fromView.Type)
        {
            case ShipType.Player:

                return shipPlayerFactory.Create(slots);

            case ShipType.Pirate:
            case ShipType.Merchant:

                return null;
                
            default:
                throw new ArgumentOutOfRangeException(nameof(fromView.Type), fromView.Type, null);
        }
    }
}