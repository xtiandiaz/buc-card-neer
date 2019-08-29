using System;
using UniRx;
using Zenject;

public interface ISlotFactory : IFactory<ISlotModel, ISlot>, IDisposable
{
    ISlot Create(ISlotModel fromModel, ISlotView andView);
}

public class SlotFactory : ISlotFactory
{
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    
    private readonly Slot.Factory instanceFactory;
    private readonly DiContainer container;
    private readonly StashSlot.Factory instanceFactoryStash;
    private readonly IBoardView boardView;

    private SlotFactory(
        DiContainer container,
        Slot.Factory instanceFactory,
        StashSlot.Factory instanceFactoryStash,
        IBoardView boardView
    )
    {
        this.container = container;
        this.instanceFactory = instanceFactory;
        this.instanceFactoryStash = instanceFactoryStash;
        this.boardView = boardView;
    }
    
    public ISlot Create(ISlotModel fromModel, ISlotView andView)
    {
        var slot = (fromModel.Type & SlotType.Stash) != 0
            ? instanceFactoryStash.Create(fromModel, (IStashSlotView) andView)
            : instanceFactory.Create(fromModel, andView);

        disposables.Add(slot);

        return slot;
    }

    public ISlot Create(ISlotModel fromModel)
    {
        var view = CreateView(fromModel);
        var slot = (fromModel.Type & SlotType.Stash) != 0
            ? instanceFactoryStash.Create(fromModel, (IStashSlotView) view)
            : instanceFactory.Create(fromModel, view);

        disposables.Add(slot);

        return slot;
    }

    public void Dispose()
    {
        disposables.Dispose();
    }

    private ISlotView CreateView(ISlotModel fromModel)
    {
        return container.InstantiatePrefabForComponent<ISlotView>(fromModel.ViewPrefab);
    }
}