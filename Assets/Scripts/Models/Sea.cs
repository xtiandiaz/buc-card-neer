using System;
using UniRx;
using Zenject;

public interface ISea : ICardProviderManager
{
    ISlot[] Slots { get; }
    
    IObservable<Unit> WhenClashed { get; }
    IObservable<IResourceCard> WhenCollected { get; }

    void Clash();
    bool CanClash(int slotAtIndex);
    void Clash(int slotAtIndex);
    void Impact(int withValue);
    void Collect();
    void Lock();
    void Unlock();
}

public class Sea : ISea
{
    public class Factory : PlaceholderFactory<ISlot[], Sea>
    {
    }

    private readonly Subject<Unit> clashing = new Subject<Unit>();
    private readonly Subject<IResourceCard> collection = new Subject<IResourceCard>();
    private readonly ICardProvider cardProvider;

    private Sea(
        ISlot[] slots,
        ICardProvider cardProvider
    )
    {
        Slots = slots;
        this.cardProvider = cardProvider;
    }

    public ISlot[] Slots { get; }

    public IObservable<Unit> WhenClashed => clashing;
    public IObservable<IResourceCard> WhenCollected => collection;

    public void AssignProviders()
    {
        foreach (var slot in Slots)
            slot.SetProvider(cardProvider);
    }

    public void Clash()
    {
        clashing.OnNext(Unit.Default);
    }

    public bool CanClash(int slotAtIndex)
    {
        if (slotAtIndex < 0 || slotAtIndex >= Slots.Length)
            throw new ArgumentOutOfRangeException();
        
        var slotToClash = Slots[slotAtIndex];
        var (previousSlot, nextSlot) = GetNeighboringSlots(slotAtIndex);

        return CanClash(slotToClash, previousSlot) || CanClash(slotToClash, nextSlot);
    }

    public void Clash(int slotAtIndex)
    {
        var slotToClash = Slots[slotAtIndex];
        var cardToClash = slotToClash.Peek();
        
        if (cardToClash == null)
            return;
        
        var (previousSlot, nextSlot) = GetNeighboringSlots(slotAtIndex);
        
        if (CanClash(slotToClash, previousSlot))
            previousSlot?.Peek()?.Clash(cardToClash, Direction.Right);
        
        if (CanClash(slotToClash, nextSlot))
            nextSlot?.Peek()?.Clash(cardToClash, Direction.Left);
    }

    public void Impact(int withValue)
    {
        foreach (var slot in Slots)
        {
            if (!CanImpact(slot))
                continue;

            slot.Peek().Impact(withValue);
        }
    }

    public void Collect()
    {
        foreach (var slot in Slots)
        {
            if (!CanCollect(slot))
                continue;

            collection.OnNext((IResourceCard) slot.Peek());
        }
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

    private bool CanClash(ISlot slot, ISlot withOther)
    {
        if (slot == null || slot.IsLocked || withOther == null || withOther.IsLocked)
            return false;
        
        var targetCard = slot.Peek();
        return targetCard != null && withOther.Peek()?.CanClash(targetCard) == true;
    }

    private bool CanImpact(ISlot slot)
    {
        return slot != null && slot.Peek()?.CanBeImpacted() == true;
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