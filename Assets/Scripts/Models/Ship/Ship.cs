using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

public interface IShip
{
    ISlot[] Slots { get; }
    ISlot BoardingSlot { get; }
    ISlot PlayerSlot { get; }
    IDictionary<ResourceType, ISlotStorage> Storage { get; }
    Vector3 Position { get; }

    IObservable<Unit> WhenDocked { get; }
    IObservable<Unit> WhenSailed { get; }
    IObservable<ICard> WhenBoarded { get; }
    IObservable<ICardResource> WhenBoardedResource { get; }
    
    void Dock(Vector3 atPosition);
    void SetSail(Vector3 toPosition);
    void Store(ICardResource card);
}

public class Ship : IShip
{
    public class Factory : PlaceholderFactory<ISlot[], Ship>
    {   
    }
    
    private readonly Subject<Unit> docking = new Subject<Unit>();
    private readonly Subject<Unit> sailing = new Subject<Unit>();
    private ICardProvider cardProvider;
    
    protected Ship(ISlot[] slots)
    {
        Slots = slots;
        BoardingSlot = Slots.FirstOrDefault(slot => slot.Type == SlotType.Boarding);
        PlayerSlot = Slots.FirstOrDefault(slot => slot.Type == SlotType.Player);
        Storage = slots.Where(slot => slot.Type == SlotType.Storage).Cast<ISlotStorage>()
            .ToDictionary(resSlot => resSlot.ResourceMask, resSlot => resSlot);
    }
    
    public ISlot[] Slots { get; }
    public ISlot BoardingSlot { get; }
    public ISlot PlayerSlot { get; }
    public IDictionary<ResourceType, ISlotStorage> Storage { get; }
    public Vector3 Position { get; private set; }

    public IObservable<Unit> WhenDocked => docking; 
    public IObservable<Unit> WhenSailed => sailing;
    public IObservable<ICard> WhenBoarded => BoardingSlot.WhenLodged;
    public IObservable<ICardResource> WhenBoardedResource =>
        WhenBoarded.Where(c => (c.Type & CardType.Resource) != 0).Cast<ICard, ICardResource>();

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

    public void Store(ICardResource card)
    {
        var slot = Storage.FirstOrDefault(s => (s.Key & card.ResourceType) != 0);
        if (slot.Value == null)
        {
            Debug.LogError($"[Ship] There's no storage Slot for Card with Resource Type {card.ResourceType}");
            return;
        }
        
       slot.Value.Lodge(card);
    }
}
