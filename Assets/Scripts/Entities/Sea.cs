using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Zenject;

public interface ISea : IInitializable, IDisposable
{
    ISlot[] Slots { get; }

    IObservable<Unit> WhenArranged { get; }
    IObservable<Unit> WhenResupplied { get; }
    
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

    private readonly Subject<Unit> arranging = new Subject<Unit>();
    private readonly Subject<Unit> resupplying = new Subject<Unit>();
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    
    private readonly ISeaView view;
    private readonly ICardDealer dealer;
    private readonly ICardClasher clasher;
    private readonly int cardCountPerSlot;

    private int clashExclusionMask;

    private Sea(
        IEnumerable<ISlot> supplySlots,
        ISeaView view,
        ICardDealer dealer,
        ICardClasher clasher,
        IBoardModel boardModel
        )
    {
        Slots = supplySlots.ToArray();
        cardCountPerSlot = boardModel.CardCountPerSupplySlot;

        this.view = view;
        this.dealer = dealer;
        this.clasher = clasher;
    }

    public ISlot[] Slots { get; }

    public IObservable<Unit> WhenArranged => arranging;
    public IObservable<Unit> WhenResupplied => resupplying;

    public void Initialize()
    {
        disposables.Add(Slots
            .Select((slot, i) => slot.WhenReleased
                .Do(_ => clashExclusionMask |= 1 << i))
            .Merge()
            .Subscribe());
    }
    
    public IObservable<Unit> Supply()
    {
        return Enumerable.Range(0, Slots.Length)
            .Select(i => dealer.Deal(cardCountPerSlot, Slots[i])
                .DelaySubscription(TimeSpan.FromSeconds(i * 0.2f)))
            .Merge()
            .AsSingleUnitObservable();
    }
    
    public IObservable<Unit> Resupply()
    {
        return Observable.Create<Unit>(observer =>
        {
            if (Slots.FirstOrDefault(slot => dealer.CanDeal(slot) && slot.IsEmpty) != null)
                resupplying.OnNext(Unit.Default);
            
            return Enumerable.Range(0, Slots.Length)
                .Select(i => Slots[i])
                .Where(slot => slot.IsEmpty)
                .Select(slot => dealer.Deal(cardCountPerSlot, slot))
                .Merge()
                .AsSingleUnitObservable()
                .Subscribe(observer);
        });
    }
    
    public IObservable<Unit> Arrange()
    {
        return Observable.Create<Unit>(observer =>
        {
            if (Slots.FirstOrDefault(slot => slot.IsMessy && !slot.IsEmpty) != null)
                arranging.OnNext(Unit.Default);

            return Slots.Select(slot => slot.ArrangeAsObservable())
                .Merge()
                .AsSingleUnitObservable()
                .Subscribe(observer);
        });
    }

    public IObservable<Unit> Clash()
    {
        return Enumerable.Range(0, Slots.Length)
            .Select(Clash)
            .Concat()
            .AsSingleUnitObservable()
            .DoOnCompleted(() => clashExclusionMask = 0);
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
    
    public void Dispose()
    {
        arranging.Dispose();
        resupplying.Dispose();
        
        disposables.Dispose();
    }

    private IObservable<Unit> Clash(int slotAtIndex)
    {
        return Observable.Create<Unit>(observer =>
        {
            if ((1 << slotAtIndex & clashExclusionMask) != 0)
                return Observable.Empty<Unit>()
                    .Subscribe(observer);
            
            var slotToClash = Slots[slotAtIndex];
            var (previousSlot, nextSlot) = GetClashingSlots(slotAtIndex);

            return clasher.Clash(previousSlot, slotToClash, Direction.Right)
                .Concat(clasher.Clash(nextSlot, slotToClash, Direction.Left))
                .Subscribe(observer);
        });
    }

    private (ISlot, ISlot) GetClashingSlots(int forIndex)
    {
        var prevIndex = forIndex - 1;
        var nextIndex = forIndex + 1;

        return (
            prevIndex >= 0 && (1 << prevIndex & clashExclusionMask) == 0 ? Slots[prevIndex] : null, 
            nextIndex < Slots.Length && (1 << nextIndex & clashExclusionMask) == 0 ? Slots[nextIndex] : null);
    }
}