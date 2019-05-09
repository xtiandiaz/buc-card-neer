using System;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

public interface IShipPlayer : IShip
{
    Vector3 DockingPosition { get; }
    
    IObservable<ICard> WhenPirateBoarded { get; }
    IObservable<ICard> WhenMerchantBoarded { get; }

    void Dock();
}

public class ShipPlayer : Ship, IShipPlayer
{
    public class Factory : PlaceholderFactory<ISlot[], ShipPlayer>
    {
    }
    
    private readonly ISlot playerSlot;

    public ShipPlayer(ISlot[] slots) : base(ShipType.Player, slots)
    {
        playerSlot = Slots.FirstOrDefault(slot => slot.Type == SlotType.Player);
    }
    
    public Vector3 DockingPosition { get; }
    
    public IObservable<ICard> WhenPirateBoarded => WhenBoarded.Where(card => (card.Type & CardType.Pirate) != 0);
    public IObservable<ICard> WhenMerchantBoarded => WhenBoarded.Where(card => (card.Type & CardType.Merchant) != 0);
    
    public override void Populate()
    {
        Feed(playerSlot);
    }

    public void Dock()
    {
        base.Dock(DockingPosition);
    }
}