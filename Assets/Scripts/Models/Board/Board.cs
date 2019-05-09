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
    IShip[] Ships { get; }
    IDeck[] Decks { get; }
    ISlot[] PlaySlots { get; }

    void Populate();
}

public class Board : IBoard
{
    public class Factory : PlaceholderFactory<ISea, IShip[], IDeck[], Board>
    {
    }
    
    private readonly IDeck eventDeck;
    private readonly IDeck resourceDeck;
    
    private Board(ISea sea, IShip[] ships, IDeck[] decks)
    {
        Sea = sea;
        Ships = ships;
        Decks = decks;

        eventDeck = Decks.FirstOrDefault(d => d.Type == DeckType.Events);
        resourceDeck = Decks.FirstOrDefault(d => d.Type == DeckType.Resources);
        
        ShipPlayer = (ShipPlayer) Ships.First(s => s.Type == ShipType.Player);
        ShipMerchant = (ShipMerchant) Ships.First(s => s.Type == ShipType.Merchant);
        ShipPirate = (ShipPirate) Ships.First(s => s.Type == ShipType.Pirate);
        
        var playSlots = ShipPlayer.Slots.ToList();
        playSlots.AddRange(Sea.Slots);
        
        PlaySlots = playSlots.ToArray();
    }

    public ISea Sea { get; }
    public IShip[] Ships { get; }
    public IDeck[] Decks { get; }
    public ISlot[] PlaySlots { get; private set; }
    
    public ShipPlayer ShipPlayer { get; private set; }
    public ShipMerchant ShipMerchant { get; private set; }
    public ShipPirate ShipPirate { get; private set; }

    public void AssignProviders()
    {
        Sea.SetProvider(eventDeck);
    }

    public void Populate()
    {
        Sea.Populate();
    }
}