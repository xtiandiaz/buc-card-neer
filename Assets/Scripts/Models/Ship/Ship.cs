using System.Collections.Generic;

public enum ShipType
{
    Player,
    Pirate,
    Merchant
}

public interface IShip
{
    ShipType Type { get; }
    ISlot[] Slots { get; }
    
    void Board(ICard card);
}

public abstract class Ship : IShip
{
    protected Ship(
        ShipType type,
        ISlot[] slots
    )
    {
        Type = type;
        Slots = slots;
    }
    
    public ShipType Type { get; }
    public ISlot[] Slots { get; }

    public abstract void Board(ICard card);
}