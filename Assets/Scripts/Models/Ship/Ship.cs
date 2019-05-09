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

public interface IShip : ICardConsumer, IBoardSection
{
    ShipType Type { get; }
    ISlot[] Slots { get; }
    ISlot BoardingSlot { get; }
    IDictionary<ResourceType, ISlotStorage> Storage { get; }

    IObservable<Vector3> WhenDocked { get; }
    IObservable<Vector3> WhenSailed { get; }
    IObservable<ICard> WhenBoarded { get; }
    
    void Dock(Vector3 atPosition);
    void SetSail(Vector3 toPosition);
    void Store(IResourceCard card);
}

public abstract class Ship : IShip
{
    private readonly Subject<Vector3> docking = new Subject<Vector3>();
    private readonly Subject<Vector3> sailing = new Subject<Vector3>();
    
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

    public IObservable<Vector3> WhenDocked => docking; 
    public IObservable<Vector3> WhenSailed => sailing;
    public IObservable<ICard> WhenBoarded => BoardingSlot.WhenLodged;

    public void Populate()
    {
        throw new NotImplementedException();
    }
    
    public void SetProvider(ICardProvider provider)
    {
        throw new NotImplementedException();
    }

    public void Feed(ISlot slot)
    {
        throw new NotImplementedException();
    }

    public void Dock(Vector3 atPosition)
    {
        docking.OnNext(atPosition);
    }

    public void SetSail(Vector3 toPosition)
    {
        sailing.OnNext(toPosition);
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

    public IObservable<ICard> WhenConsumed { get; }
}
