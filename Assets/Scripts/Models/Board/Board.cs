using System.Linq;
using Zenject;

public enum BoardMode
{
    Seafaring,
    Trade,
    Combat
}

public interface IBoard : ICardProviderManager
{
    ISea Sea { get; }
    IShipPlayer ShipPlayer { get; }

    void Populate();
}

public class Board : IBoard
{
    public class Factory : PlaceholderFactory<ISea, IShip[], IDeck[], Board>
    {
    }
    
    private readonly IDeck eventDeck;
    private readonly IDeck resourceDeck;
    [Inject] private IPlayerProvider playerProvider;
    
    private Board(ISea sea, IShip[] ships, IDeck[] decks)
    {
        Sea = sea;

        eventDeck = decks.FirstOrDefault(d => d.Type == DeckType.Events);
        resourceDeck = decks.FirstOrDefault(d => d.Type == DeckType.Resources);
        
        ShipPlayer = (ShipPlayer) ships.First(s => s.Type == ShipType.Player);
        ShipMerchant = (ShipMerchant) ships.First(s => s.Type == ShipType.Merchant);
        ShipPirate = (ShipPirate) ships.First(s => s.Type == ShipType.Pirate);
    }

    public ISea Sea { get; }
    
    public IShipPlayer ShipPlayer { get; private set; }
    public ShipMerchant ShipMerchant { get; private set; }
    public ShipPirate ShipPirate { get; private set; }

    public void AssignProviders()
    {
        Sea.SetProvider(eventDeck);
        ShipPlayer.SetProvider(playerProvider);
    }

    public void Populate()
    {
        Sea.Populate();
        ShipPlayer.Populate();
    }
}