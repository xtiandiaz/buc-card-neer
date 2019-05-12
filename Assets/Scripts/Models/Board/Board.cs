using System.Linq;
using Zenject;

public enum BoardMode
{
    Seafaring,
    Trade,
    Combat
}

public interface IBoard
{
    ISea Sea { get; }
    IShipPlayer ShipPlayer { get; }
}

public class Board : IBoard
{
    public class Factory : PlaceholderFactory<Board>
    {
    }
    
    private Board()
    {
        /*Sea = sea;
        
        ShipPlayer = (ShipPlayer) ships.First(s => s.Type == ShipType.Player);
        ShipMerchant = (ShipMerchant) ships.First(s => s.Type == ShipType.Merchant);
        ShipPirate = (ShipPirate) ships.First(s => s.Type == ShipType.Pirate);*/
    }

    public ISea Sea { get; }
    
    public IShipPlayer ShipPlayer { get; private set; }
    public ShipMerchant ShipMerchant { get; private set; }
    public ShipPirate ShipPirate { get; private set; }
}