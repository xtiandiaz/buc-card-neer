using System.Collections.Generic;
using System.Linq;
using Zenject;

public class ShipPlayer : Ship
{
    public class Factory : PlaceholderFactory<ISlot[], ShipPlayer>
    {
    }

    public ShipPlayer(ISlot[] slots) : base(ShipType.Player, slots)
    {
        BoardingSlot = Slots.FirstOrDefault(s => s.Type == SlotType.Boarding);
    }
    
    public ISlot BoardingSlot { get; }
}