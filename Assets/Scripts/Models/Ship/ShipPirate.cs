using Zenject;

public class ShipPirate : Ship
{
    public class Factory : PlaceholderFactory<ISlot[], ShipPirate>
    {
    }

    public ShipPirate(ISlot[] slots) : base(ShipType.Pirate, slots)
    {
    }

    public override void Populate()
    {
        throw new System.NotImplementedException();
    }
}