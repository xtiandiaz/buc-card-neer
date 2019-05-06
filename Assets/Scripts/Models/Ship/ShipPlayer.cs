using System;
using System.Linq;
using UniRx;
using Zenject;

public interface IShipPlayer : IShip
{
    ISlot PlayerSlot { get; }
    IObservable<ICard> PirateBoarding { get; }
    IObservable<ICard> MerchantBoarding { get; }
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
    public IObservable<ICard> PirateBoarding => Boarding.Where(card => (card.Type & CardType.Pirate) != 0);
    public IObservable<ICard> MerchantBoarding => Boarding.Where(card => (card.Type & CardType.Merchant) != 0);
}