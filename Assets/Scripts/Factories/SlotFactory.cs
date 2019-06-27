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
    private readonly ICardRouter cardRouter;

    private SlotFactory(
        Slot.Factory slotFactory,
        ICardRouter cardRouter
    )
    {
        this.slotFactory = slotFactory;
        this.cardRouter = cardRouter;
    }
    
    public ISlot Create(ISlotModel fromModel, ISlotView andView)
    {
        var slot = slotFactory.Create(fromModel, andView);
        
        cardRouter.Register(slot);
        
        disposables.Add(slot);

        return slot;
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}