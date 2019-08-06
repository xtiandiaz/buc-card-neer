using System;
using UniRx;
using Zenject;

public interface ISlotFactory : IFactory<ISlotModel, ISlotView, ISlot>, IDisposable
{
}

public class SlotFactory : ISlotFactory
{
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    
    private readonly Slot.Factory slotFactory;
    private readonly StashSlot.Factory stashFactory;

    private SlotFactory(
        Slot.Factory slotFactory,
        StashSlot.Factory stashFactory
    )
    {
        this.slotFactory = slotFactory;
        this.stashFactory = stashFactory;
    }
    
    public ISlot Create(ISlotModel fromModel, ISlotView andView)
    {
        var slot = (fromModel.Type & SlotType.Stash) != 0
            ? stashFactory.Create(fromModel, (IStashSlotView) andView)
            : slotFactory.Create(fromModel, andView);

        disposables.Add(slot);

        return slot;
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}