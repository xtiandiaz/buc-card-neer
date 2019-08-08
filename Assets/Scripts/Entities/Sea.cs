using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Zenject;

public interface ISea
{
    ISlot[] Slots { get; }
    
    bool ShouldArrange { get; }
    bool ShouldResupply { get; }

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

    private readonly ISlot[] slots;
    private readonly ISeaView view;
    private readonly ICardDealer dealer;
    private readonly ICardClasher clasher;
    private readonly int cardCountPerSlot;

    private Sea(
        IEnumerable<ISlot> supplySlots,
        ISeaView view,
        ICardDealer dealer,
        ICardClasher clasher,
        IBoardModel boardModel
        )
    {
        slots = supplySlots.ToArray();
        cardCountPerSlot = boardModel.CardCountPerSupplySlot;

        this.view = view;
        this.dealer = dealer;
        this.clasher = clasher;
    }

    public ISlot[] Slots => slots;

    public bool ShouldArrange => slots.FirstOrDefault(slot => slot.IsMessy && !slot.IsEmpty) != null;
    public bool ShouldResupply => slots.FirstOrDefault(slot => dealer.CanDeal(slot) && slot.IsEmpty) != null;
    
    public IObservable<Unit> Supply()
    {
        return Enumerable.Range(0, slots.Length)
            .Select(i => dealer.Deal(cardCountPerSlot, slots[i])
                .DelaySubscription(TimeSpan.FromSeconds(i * 0.2f)))
            .Merge()
            .AsSingleUnitObservable();
    }
    
    public IObservable<Unit> Resupply()
    {
        return Enumerable.Range(0, slots.Length)
            .Select(i => slots[i])
            .Where(slot => slot.IsEmpty)
            .Select(slot => dealer.Deal(cardCountPerSlot, slot))
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
            .AsSingleUnitObservable();
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