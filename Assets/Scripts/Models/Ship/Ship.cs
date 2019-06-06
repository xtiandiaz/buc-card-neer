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
    IObservable<ICard> WhenMatched { get; }
    IObservable<int> WhenShot { get; }
    
    IObservable<IResourceCard> BoardAndStore(IResourceCard card);
    void Lock();
    void Unlock();
}

public class Ship : IShip
{
    public class Factory : PlaceholderFactory<ISlot[], Ship>
    {
    }
    
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
        .SelectMany(card =>
        {
            if (card is IResourceCard resource)
                return BoardAndStore(resource);
            
            return Board(card);
        });

    public IObservable<ICard> WhenMatched => Slots
        .Select(slot => slot.WhenMatched)
        .Merge();

    public IObservable<int> WhenShot => boardingSlot.WhenLodged
        .Where(card => (card.Type & CardType.WeaponRanged) != 0 && card.IsStored)
        .Do(_ => Lock())
        .Delay(TimeSpan.FromSeconds(0.5))
        .Select(Shoot);
    
    public IObservable<IResourceCard> BoardAndStore(IResourceCard resource)
    {
        return Board(resource)
            .Select(_ => resource)
            .Delay(TimeSpan.FromSeconds(0.3))
            .Do(Store);
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
    
    private IObservable<ICard> Board(ICard card)
    {
        return Observable.Create<ICard>(observer =>
        {
            card.IsBoarded = true;

            if (card is IResourceCard resource && resource.IsLocked)
                return resource.WhenUnlocked
                    .Select(_ => resource)
                    .Do(_ => resource.Flip(CardFace.Front))
                    .Subscribe(observer);

            card.Flip(CardFace.Front);

            observer.OnNext(card);
            observer.OnCompleted();

            return Disposable.Create(() => { });
        });
    }
    
    private void Store(IResourceCard card)
    {
        if (card.IsStored)
        {
            Debug.LogError("[Ship] Apparently, the Card is already stored.");
            return;
        }
        
        card.IsStored = true;

        var slot = storage.FirstOrDefault(s => (s.Key & card.ResourceType) != 0);
        if (slot.Value == null)
        {
            Debug.LogError($"[Ship] There's no storage Slot for Card with Resource Type {card.ResourceType}");
            return;
        }

        slot.Value.Lodge(card);
    }
    
    private int Shoot(ICard withCard)
    {
        if ((withCard.Type & CardType.WeaponRanged) == 0)
            return 0;

        var weaponValue = withCard.Value;

        withCard.Destroy();

        return weaponValue;
    }
}
