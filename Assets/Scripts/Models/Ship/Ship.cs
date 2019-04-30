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
    
    IObservable<ICard> Boarded { get; }
    IObservable<(ICard, ISlot)> Lodged { get; }
    IObservable<Vector3> Docked { get; }
    IObservable<Vector3> Sailed { get; }

    void Dock(Vector3 atPosition);
    void SetSail(Vector3 toPosition);
}

public abstract class Ship : IShip
{
    private readonly Subject<Vector3> docked = new Subject<Vector3>();
    private readonly Subject<Vector3> sailed = new Subject<Vector3>();
    
    protected Ship(ShipType type, ISlot[] slots)
    {
        Type = type;
        Slots = slots;
    }
    
    public ShipType Type { get; }
    public ISlot[] Slots { get; }

    public IObservable<(ICard, ISlot)> Lodged => Slots.Select(slot => slot.Lodged.Select(card => (card, slot))).Merge();
    public IObservable<ICard> Boarded => Slots.Where(s => s.Type == SlotType.Boarding).Select(s => s.Lodged).Merge();
    public IObservable<Vector3> Docked => docked; 
    public IObservable<Vector3> Sailed => sailed; 
    
    public void Dock(Vector3 atPosition)
    {
        docked.OnNext(atPosition);
    }

    public void SetSail(Vector3 toPosition)
    {
        sailed.OnNext(toPosition);
    }
}
