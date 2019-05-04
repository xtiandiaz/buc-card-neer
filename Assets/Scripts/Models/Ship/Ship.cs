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
    IDictionary<ResourceType, ISlotStorage> Storage { get; }

    IObservable<Vector3> Docked { get; }
    IObservable<Vector3> Sailed { get; }
    IObservable<ICard> Boarded { get; }

    void Dock(Vector3 atPosition);
    void SetSail(Vector3 toPosition);
    void Store(IResourceCard card);
}

public abstract class Ship : IShip
{
    private readonly Subject<Vector3> docked = new Subject<Vector3>();
    private readonly Subject<Vector3> sailed = new Subject<Vector3>();
    
    protected Ship(ShipType type, ISlot[] slots)
    {
        Type = type;
        Slots = slots;
        Storage = slots.Where(slot => slot.Type == SlotType.Storage).Cast<ISlotStorage>()
            .ToDictionary(resSlot => resSlot.ResourceMask, resSlot => resSlot);
    }
    
    public ShipType Type { get; }
    public ISlot[] Slots { get; }
    public IDictionary<ResourceType, ISlotStorage> Storage { get; }

    public IObservable<Vector3> Docked => docked; 
    public IObservable<Vector3> Sailed => sailed; 
    public IObservable<ICard> Boarded => Slots.Where(s => s.Type == SlotType.Boarding).Select(s => s.Taking).Merge();
    
    public void Dock(Vector3 atPosition)
    {
        docked.OnNext(atPosition);
    }

    public void SetSail(Vector3 toPosition)
    {
        sailed.OnNext(toPosition);
    }

    public void Store(IResourceCard card)
    {
        if (!Storage.ContainsKey(card.ResourceType))
        {
            Debug.LogError($"[Ship] There's no storage Slot for Card with Resource Type {card.ResourceType}");
            return;
        }
        
        Storage[card.ResourceType].Take(card);
    }
}
