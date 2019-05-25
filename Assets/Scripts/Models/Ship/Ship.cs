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
    IDictionary<ResourceType, IStorageSlot> Storage { get; }
    Vector3 Position { get; }
    
    IObservable<ICard> WhenBoarded { get; }
    IObservable<IResourceCard> WhenBoardedResource { get; }

    void Store(IResourceCard card);
}

public class Ship : IShip
{
    public class Factory : PlaceholderFactory<ISlot[], Ship>
    {   
    }
    
    private ICardProvider cardProvider;
    
    protected Ship(ISlot[] slots)
    {
        Slots = slots;
        BoardingSlot = Slots.FirstOrDefault(slot => slot.Type == SlotType.Boarding);
        PlayerSlot = Slots.FirstOrDefault(slot => slot.Type == SlotType.Player);
        Storage = slots.Where(slot => slot.Type == SlotType.Storage).Cast<IStorageSlot>()
            .ToDictionary(resSlot => resSlot.ResourceMask, resSlot => resSlot);
    }
    
    public ISlot[] Slots { get; }
    public ISlot BoardingSlot { get; }
    public ISlot PlayerSlot { get; }
    public IDictionary<ResourceType, IStorageSlot> Storage { get; }
    public Vector3 Position { get; private set; }
    
    public IObservable<ICard> WhenBoarded => BoardingSlot.WhenLodged;
    public IObservable<IResourceCard> WhenBoardedResource =>
        WhenBoarded.Where(c => (c.Type & CardType.Resource) != 0).Cast<ICard, IResourceCard>();

    public void Store(IResourceCard card)
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
