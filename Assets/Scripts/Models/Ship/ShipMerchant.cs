using Zenject;

public class ShipMerchant : Ship
{
    public class Factory : PlaceholderFactory<ISlot[], ShipMerchant>
    {
    }

    public ShipMerchant(ISlot[] slots) : base(ShipType.Merchant, slots)
    {
    }
}