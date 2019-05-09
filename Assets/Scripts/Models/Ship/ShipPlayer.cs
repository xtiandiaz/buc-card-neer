using System;
using System.Linq;
using UniRx;
using Zenject;

public interface IShipPlayer : IShip
{
    ISlot PlayerSlot { get; }
    
    IObservable<ICard> WhenPirateBoarded { get; }
    IObservable<ICard> WhenMerchantBoarded { get; }
}

public class ShipPlayer : Ship, IShipPlayer
{
    public class Factory : PlaceholderFactory<ISlot[], ShipPlayer>
    {
    }
    
    private readonly ICardPlayer playerCard;

    public ShipPlayer(ISlot[] slots) : base(ShipType.Player, slots)
    {
        PlayerSlot = Slots.FirstOrDefault(slot => slot.Type == SlotType.Player);
    }

    public ISlot PlayerSlot { get; }
    
    public IObservable<ICard> WhenPirateBoarded => WhenBoarded.Where(card => (card.Type & CardType.Pirate) != 0);
    public IObservable<ICard> WhenMerchantBoarded => WhenBoarded.Where(card => (card.Type & CardType.Merchant) != 0);
}