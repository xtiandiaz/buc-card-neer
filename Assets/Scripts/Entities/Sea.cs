using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Zenject;

public interface ISea
{
    ISlot[] Slots { get; }
    
    bool IsMessy { get; }
    bool ShouldResupply { get; }
    
    IObservable<ISlot> WhenReleasedSupply { get; }
    
    void Lock();
    void Unlock();
    
    IObservable<Unit> Supply();
    IObservable<Unit> Clash();
    IObservable<Unit> Resupply();
    IObservable<Unit> Arrange();
}

public class Sea : ISea
{
    public class Factory : PlaceholderFactory<IEnumerable<ISlot>, ISeaView, Sea>
    {
    }
    
    private const int CardCountPerSlot = 3;

    private readonly ISlot[] slots;
    private readonly ISeaView view;
    private readonly ICardDealer dealer;
    private readonly ICardClasher clasher;

    private Sea(
        IEnumerable<ISlot> supplySlots,
        ISeaView view,
        ICardDealer dealer,
        ICardClasher clasher
        )
    {
        slots = supplySlots.ToArray();

        this.view = view;
        this.dealer = dealer;
        this.clasher = clasher;
    }

    public ISlot[] Slots => slots;

    public bool IsMessy => slots.FirstOrDefault(slot => slot.IsMessy) != null;
    public bool ShouldResupply => slots.FirstOrDefault(slot => dealer.CanDeal(slot) && slot.IsEmpty) != null;

    public IObservable<ISlot> WhenReleasedSupply => slots
        .Select(slot => slot.WhenReleased.Select(_ => slot))
        .Merge();

    public IObservable<Unit> Supply()
    {
        return Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(0.075))
            .Take(slots.Length * CardCountPerSlot)
            .SelectMany(i => dealer.DealOne(slots[i % slots.Length]))
            .AsSingleUnitObservable();
    }
    
    public IObservable<Unit> Resupply()
    {
        return Enumerable.Range(0, slots.Length)
            .Select(i => slots[i])
            .Where(slot => slot.IsEmpty)
            .Select(slot => dealer.Deal(CardCountPerSlot, slot))
            .Concat()
            .AsSingleUnitObservable();
    }

    public IObservable<Unit> Clash()
    {
        return Enumerable.Range(0, slots.Length)
            .Select(Clash)
            .Concat()
            .AsSingleUnitObservable();
    }

    public IObservable<Unit> Arrange()
    {
        return slots.Select(slot => slot.Arrange())
            .Merge()
            .AsUnitObservable();
    }
    
    public void Lock()
    {
        foreach (var slot in slots)
            slot.Lock();
    }

    public void Unlock()
    {
        foreach (var slot in slots)
            slot.Unlock();
    }

    private IObservable<Unit> Clash(int slotAtIndex)
    {
        return Observable.Create<Unit>(observer =>
        {
            var slotToClash = slots[slotAtIndex];
            var (previousSlot, nextSlot) = GetNeighboringSlots(slotAtIndex);

            return clasher.Clash(previousSlot, slotToClash, Direction.Right)
                .Concat(clasher.Clash(nextSlot, slotToClash, Direction.Left))
                .Subscribe(observer);
        });
    }

    private (ISlot, ISlot) GetNeighboringSlots(int atIndex)
    {
        return (atIndex - 1 >= 0 ? slots[atIndex - 1] : null, atIndex + 1 < slots.Length ? slots[atIndex + 1] : null);
    }
}