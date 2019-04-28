using System.Collections.Generic;

public enum ShipType
{
    Player,
    Pirate,
    Merchant
}

public interface IShip
{
    IEnumerable<ISlot> Slots { get; }
    
    void Board(ICard card);
}

public abstract class Ship : IShip
{
    protected Ship(
        IEnumerable<ISlot> slots
    )
    {
        Slots = slots;
    }
    
    public IEnumerable<ISlot> Slots { get; }

    public abstract void Board(ICard card);
}