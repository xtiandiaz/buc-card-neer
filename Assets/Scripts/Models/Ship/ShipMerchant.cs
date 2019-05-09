using Zenject;

public class ShipMerchant : Ship
{
    public class Factory : PlaceholderFactory<ISlot[], ShipMerchant>
    {
    }

    public ShipMerchant(ISlot[] slots) : base(ShipType.Merchant, slots)
    {
    }

    public override void Populate()
    {
        throw new System.NotImplementedException();
    }
}