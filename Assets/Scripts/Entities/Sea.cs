using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Zenject;

public interface ISea
{
    ISlot[] Slots { get; }
    
    IObservable<ISlot> WhenReleasedSupply { get; }
    
    void Impact(int withValue);
    void Lock();
    void Unlock();
    
    IObservable<Unit> Supply();
    IObservable<Unit> Clash();
    IObservable<ICard> Collect();
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

    public IObservable<ISlot> WhenReleasedSupply => slots
        .Select(slot => slot.WhenReleased.Select(_ => slot))
        .Merge();

    public IObservable<Unit> Supply()
    {
        return Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(0.1))
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

    public void Impact(int withValue)
    {
        foreach (var slot in slots)
        {
            if (!CanImpact(slot))
                continue;

            //slot.Peek().Hit(withValue);
        }
    }

    public IObservable<ICard> Collect()
    {
        return Observable.Create<ICard>(observer =>
        {
            foreach (var slot in slots)
            {
                if (!CanCollect(slot))
                    continue;

                observer.OnNext(slot.Peek());
            }

            observer.OnCompleted();

            return Disposable.Empty;
        });
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

    private bool CanImpact(ISlot slot)
    {
        return slot != null && slot.Peek()?.IsRangeTarget == true;
    }

    private bool CanCollect(ISlot fromSlot)
    {
        var card = fromSlot?.Peek();
        
        return card?.IsResource == true && card.WasLocked && !card.IsLocked;
    }

    private (ISlot, ISlot) GetNeighboringSlots(int atIndex)
    {
        return (atIndex - 1 >= 0 ? slots[atIndex - 1] : null, atIndex + 1 < slots.Length ? slots[atIndex + 1] : null);
    }
}