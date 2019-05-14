using System;
using System.Linq;
using UniRx;
using Zenject;

public interface IShipFactory : IFactory<ShipType, IShip>, IDisposable
{
}

public class ShipFactory : IShipFactory
{
    private readonly ShipPlayer.Factory modelFactoryPlayer;
    private readonly ShipPlayerView.Factory viewFactoryPlayer;
    private readonly ShipController.Factory controllerFactory;
    private readonly ShipPlayerController.Factory controllerFactoryPlayer;
    private readonly ISlotFactory slotFactory;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private ShipFactory(
        ShipPlayer.Factory modelFactoryPlayer,
        ShipPlayerView.Factory viewFactoryPlayer,
        ShipController.Factory controllerFactory,
        ShipPlayerController.Factory controllerFactoryPlayer,
        ISlotFactory slotFactory
        )
    {
        this.modelFactoryPlayer = modelFactoryPlayer;
        this.viewFactoryPlayer = viewFactoryPlayer;        
        this.controllerFactory = controllerFactory;
        this.controllerFactoryPlayer = controllerFactoryPlayer;
        this.slotFactory = slotFactory;
    }
    
    public IShip Create(ShipType forType)
    {
        var view = CreateView(forType);
        var slots = view.Slots.Select(slotFactory.Create).ToArray();
        var model = CreateModel(forType, slots);
        
        disposables.Add(CreateController(model, view));

        return model;
    }

    private IShip CreateModel(ShipType forType, ISlot[] withSlots)
    {
        switch (forType)
        {
            case ShipType.Player:

                return modelFactoryPlayer.Create(withSlots);
                
            default:
                throw new ArgumentOutOfRangeException(nameof(forType), forType, null);
        }
    }

    private IShipView CreateView(ShipType forType)
    {
        switch (forType)
        {
            case ShipType.Player:

                return viewFactoryPlayer.Create();
                
            default:
                throw new ArgumentOutOfRangeException(nameof(forType), forType, null);
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

    public void Dispose()
    {
        disposables?.Dispose();
    }
}