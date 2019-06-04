using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

public interface IShip
{
    ISlot[] Slots { get; }
    ISlot PlayerSlot { get; }

    IObservable<ICard> WhenBoarded { get; }
    IObservable<Unit> WhenArmed { get; }
    IObservable<int> WhenShot { get; }

    void Shoot();
    void Store(IResourceCard card);
    void Lock();
    void Unlock();
}

public class Ship : IShip
{
    public class Factory : PlaceholderFactory<ISlot[], Ship>
    {
    }

    private readonly Subject<int> shooting = new Subject<int>();
    private readonly ISlot boardingSlot;
    private readonly IDictionary<ResourceType, IStorageSlot> storage;
    private ICardProvider cardProvider;

    protected Ship(ISlot[] slots)
    {
        Slots = slots;
        boardingSlot = Slots.FirstOrDefault(slot => slot.Type == SlotType.Boarding);
        storage = slots.Where(slot => slot.Type == SlotType.Storage).Cast<IStorageSlot>()
            .ToDictionary(resSlot => resSlot.ResourceMask, resSlot => resSlot);

        PlayerSlot = Slots.FirstOrDefault(slot => slot.Type == SlotType.Player);
    }

    public ISlot[] Slots { get; }
    public ISlot PlayerSlot { get; }

    public IObservable<ICard> WhenBoarded => boardingSlot.WhenLodged
        .Where(card => !card.IsBoarded)
        .Do(card => card.IsBoarded = true);

    public IObservable<Unit> WhenArmed => boardingSlot.WhenLodged
        .Where(card => (card.Type & CardType.WeaponRanged) != 0 && card.IsStored)
        .AsUnitObservable();

    public IObservable<int> WhenShot => shooting;

    public void Shoot()
    {
        var weapon = boardingSlot.Peek();
        if ((weapon.Type & CardType.WeaponRanged) == 0)
            return;

        shooting.OnNext(weapon.Value);

        weapon.Destroy();
    }

    public void Store(IResourceCard card)
    {
        card.IsStored = true;

        var slot = storage.FirstOrDefault(s => (s.Key & card.ResourceType) != 0);
        if (slot.Value == null)
        {
            Debug.LogError($"[Ship] There's no storage Slot for Card with Resource Type {card.ResourceType}");
            return;
        }

        slot.Value.Lodge(card);
    }

    public void Lock()
    {
        foreach (var slot in Slots)
            slot.Lock();
    }

    public void Unlock()
    {
        foreach (var slot in Slots)
        {
            if ((slot.Type & SlotType.Player) != 0)
                continue;

            slot.Unlock();
        }
    }
}
