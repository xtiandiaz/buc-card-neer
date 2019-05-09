using System;
using System.Linq;
using Zenject;

public interface IShipFactory : IFactory<IShipView, IShip>
{
    IShip Create(IShipView fromView);
}

public class ShipFactory : IShipFactory
{
    private readonly ShipPlayer.Factory modelFactoryPlayer;
    private readonly ShipMerchant.Factory modelFactoryMerchant;
    private readonly ShipPirate.Factory modelFactoryPirate;
    private readonly ShipController.Factory controllerFactory;
    private readonly ShipPlayerController.Factory controllerFactoryPlayer;
    private readonly ISlotFactory slotFactory;
    private readonly ICardFactory cardFactory;
    private readonly ICardPlayer cardPlayer;

    private ShipFactory(
        ShipPlayer.Factory modelFactoryPlayer,
        ShipMerchant.Factory modelFactoryMerchant,
        ShipPirate.Factory modelFactoryPirate,    
        ShipController.Factory controllerFactory,
        ShipPlayerController.Factory controllerFactoryPlayer,
        ISlotFactory slotFactory
        )
    {
        this.modelFactoryPlayer = modelFactoryPlayer;
        this.modelFactoryMerchant = modelFactoryMerchant;
        this.modelFactoryPirate = modelFactoryPirate;
        
        this.controllerFactory = controllerFactory;
        this.controllerFactoryPlayer = controllerFactoryPlayer;
        
        this.slotFactory = slotFactory;
    }
    
    public IShip Create(IShipView fromView)
    {
        var model = CreateModel(fromView);
        
        CreateController(model, fromView);

        return model;
    }

    private IShip CreateModel(IShipView fromView)
    {
        var slots = fromView.Slots.Select(sv => slotFactory.Create(sv)).ToArray();
        
        switch (fromView.Type)
        {
            case ShipType.Player:

                return modelFactoryPlayer.Create(slots);

            case ShipType.Pirate:

                return modelFactoryPirate.Create(slots);
                
            case ShipType.Merchant:

                return modelFactoryMerchant.Create(slots);
                
            default:
                throw new ArgumentOutOfRangeException(nameof(fromView.Type), fromView.Type, null);
        }
    }

    private IShipController CreateController(IShip withModel, IShipView andView)
    {
        switch (withModel.Type)
        {
            case ShipType.Player:
                
                return controllerFactoryPlayer.Create((IShipPlayer) withModel, (ShipPlayerView) andView);

            default:

                return controllerFactory.Create(withModel, andView);
        }
    }
}