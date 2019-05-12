using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

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
    ISlot BoardingSlot { get; }
    IDictionary<ResourceType, ISlotStorage> Storage { get; }
    Vector3 Position { get; }

    IObservable<Unit> WhenDocked { get; }
    IObservable<Unit> WhenSailed { get; }
    IObservable<ICard> WhenBoarded { get; }
    
    void Dock(Vector3 atPosition);
    void SetSail(Vector3 toPosition);
    void Store(IResourceCard card);
}

public abstract class Ship : IShip
{
    private readonly Subject<Unit> docking = new Subject<Unit>();
    private readonly Subject<Unit> sailing = new Subject<Unit>();
    private readonly Subject<ICard> consumption = new Subject<ICard>();
    private ICardProvider cardProvider;
    
    protected Ship(ShipType type, ISlot[] slots)
    {
        Type = type;
        Slots = slots;
        BoardingSlot = Slots.FirstOrDefault(slot => slot.Type == SlotType.Boarding);
        Storage = slots.Where(slot => slot.Type == SlotType.Storage).Cast<ISlotStorage>()
            .ToDictionary(resSlot => resSlot.ResourceMask, resSlot => resSlot);
    }
    
    public ShipType Type { get; }
    public ISlot[] Slots { get; }
    public ISlot BoardingSlot { get; }
    public IDictionary<ResourceType, ISlotStorage> Storage { get; }
    public Vector3 Position { get; private set; }

    public IObservable<Unit> WhenDocked => docking; 
    public IObservable<Unit> WhenSailed => sailing;
    public IObservable<ICard> WhenBoarded => BoardingSlot.WhenLodged;
    public IObservable<ICard> WhenConsumed => consumption;

    public void Dock(Vector3 atPosition)
    {
        Position = atPosition;
        
        docking.OnNext(Unit.Default);
    }

    public void SetSail(Vector3 toPosition)
    {
        Position = toPosition;
        
        sailing.OnNext(Unit.Default);
    }

    public void Store(IResourceCard card)
    {
        if (!Storage.ContainsKey(card.ResourceType))
        {
            Debug.LogError($"[Ship] There's no storage Slot for Card with Resource Type {card.ResourceType}");
            return;
        }
        
        Storage[card.ResourceType].Lodge(card);
    }
}
