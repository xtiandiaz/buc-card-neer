using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

public interface ISea : ICardProviderManager
{
    ISlot[] Slots { get; }

    IObservable<Unit> Clash();
    void Impact(int withValue);
    IObservable<IResourceCard> Collect();
    IObservable<Unit> Supply();
    IObservable<Unit> Resupply();
    void Arrange();
    void Lock();
    void Unlock();
}

public class Sea : ISea
{
    public class Factory : PlaceholderFactory<ISlot[], Sea>
    {
    }

    private const int CardCountPerSlot = 3;
    
    private readonly ICardProvider cardProvider;

    private Sea(ISlot[] slots, ICardProvider cardProvider)
    {
        Slots = slots;
        this.cardProvider = cardProvider;
    }

    public ISlot[] Slots { get; }

    public void AssignProviders()
    {
        foreach (var slot in Slots)
        {
            slot.SetProvider(cardProvider);
            slot.SetCapacity(CardCountPerSlot);
        }
    }

    public IObservable<Unit> Clash()
    {
        return Observable.Create<Unit>(observer =>
        {
            var indicesToClash = Enumerable.Range(0, Slots.Length).Where(CanClash).ToArray();

            if (indicesToClash.Length > 0)
            {
                return indicesToClash
                    .Select(Clash)
                    .Concat()
                    .LastOrDefault()
                    .Subscribe(observer);
            }
            
            observer.OnNext(Unit.Default);
            observer.OnCompleted();
            
            return Disposable.Create(() => { });
        });
    }

    public void Impact(int withValue)
    {
        foreach (var slot in Slots)
        {
            if (!CanImpact(slot))
                continue;

            slot.Peek().Strike(withValue, PlayerAttackType.Ranged);
        }
    }

    public IObservable<IResourceCard> Collect()
    {
        return Observable.Create<IResourceCard>(observer =>
        {
            foreach (var slot in Slots)
            {
                if (!CanCollect(slot))
                    continue;

                observer.OnNext((IResourceCard) slot.Peek());
            }
            
            observer.OnCompleted();
            
            return Disposable.Empty;
        });
    }

    public IObservable<Unit> Supply()
    {
        return Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(0.1))
            .Take(Slots.Length * CardCountPerSlot)
            .Do(i => Slots[i % Slots.Length].Consume())
            .AsSingleUnitObservable();
    }

    public IObservable<Unit> Resupply()
    {
        return Slots
            .Where(slot => slot.IsEmpty)
            .Select(slot => slot.FillToCapacity(TimeSpan.FromSeconds(0.1)))
            .Concat()
            .AsSingleUnitObservable();
    }
    
    public void Arrange()
    {
        foreach (var slot in Slots)
            slot.Arrange();
    }
    
    public void Lock()
    {
        foreach (var slot in Slots)
            slot.Lock();
    }

    public void Unlock()
    {
        foreach (var slot in Slots)
            slot.Unlock();
    }
    
    private bool CanClash(int slotAtIndex)
    {
        if (slotAtIndex < 0 || slotAtIndex >= Slots.Length)
            throw new ArgumentOutOfRangeException();
        
        var slotToClash = Slots[slotAtIndex];
        var (previousSlot, nextSlot) = GetNeighboringSlots(slotAtIndex);

        return CanClash(slotToClash, previousSlot) || CanClash(slotToClash, nextSlot);
    }

    private IObservable<Unit> Clash(int slotAtIndex)
    {
        return Observable.Create<Unit>(observer =>
        {
            var slotToClash = Slots[slotAtIndex];
            var cardToClash = slotToClash.Peek();

            if (cardToClash != null)
            {
                var (previousSlot, nextSlot) = GetNeighboringSlots(slotAtIndex);
                var clash = new List<IObservable<Unit>>();
                
                if (CanClash(slotToClash, previousSlot))
                    clash.Add(previousSlot?.Peek()?.Clash(cardToClash, Direction.Right));
                
                if (CanClash(slotToClash, nextSlot))
                    clash.Add(nextSlot?.Peek()?.Clash(cardToClash, Direction.Left));

                clash.Concat()
                    .LastOrDefault()
                    .Subscribe(observer);
            }
            else
                observer.OnCompleted();

            return Disposable.Empty;
        });
    }

    private bool CanClash(ISlot slot, ISlot withOther)
    {
        if (slot == null || slot.IsLocked || withOther == null || withOther.IsLocked)
            return false;
        
        var targetCard = slot.Peek();
        return targetCard != null && withOther.Peek()?.CanClash(targetCard) == true;
    }

    private bool CanImpact(ISlot slot)
    {
        return slot != null && slot.Peek()?.CanBeStruck() == true;
    }

    private bool CanCollect(ISlot fromSlot)
    {
        return fromSlot?.Peek() is IResourceCard resourceCard && resourceCard.WasLocked && !resourceCard.IsLocked;
    }

    private (ISlot, ISlot) GetNeighboringSlots(int atIndex)
    {
        return (atIndex - 1 >= 0 ? Slots[atIndex - 1] : null, atIndex + 1 < Slots.Length ? Slots[atIndex + 1] : null);
    }
}