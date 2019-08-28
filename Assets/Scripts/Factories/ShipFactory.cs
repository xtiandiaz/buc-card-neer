using System.Collections.Generic;
using System.Linq;
using UniRx;
using Zenject;

public interface IShipFactory : IFactory<IShip>
{
}

public class ShipFactory : IShipFactory
{
    private readonly Ship.Factory instanceFactory;
    private readonly IBoardView boardView;
    private readonly ISlotFactory slotFactory;
    private readonly List<ISlotModel> slotData;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private ShipFactory(
        Ship.Factory instanceFactory,
        IBoardView boardView,
        ISlotFactory slotFactory,
        List<ISlotModel> slotData
        )
    {
        this.instanceFactory = instanceFactory;
        this.boardView = boardView;
        this.slotFactory = slotFactory;
        this.slotData = slotData;
    }
    
    public IShip Create()
    {
        var view = boardView.Ship;
        
        var slots = slotData
            .Select(data => slotFactory.Create(data, view.Slots.First(slotView => slotView.Type == data.Type)))
            .ToDictionary(slot => slot.Type);

        return instanceFactory.Create(
            slots[SlotType.Player],
            slots[SlotType.Boarding], 
            slots[SlotType.Storage], 
            slots[SlotType.Mount],
            view);
    }
}