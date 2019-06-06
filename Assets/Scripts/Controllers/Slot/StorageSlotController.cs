using UniRx;
using Zenject;

public class StorageSlotController : SlotController
{
    public class Factory : PlaceholderFactory<IStorageSlot, IStorageSlotView, StorageSlotController>
    {
    }

    private readonly IStorageSlot model;
    private readonly IStorageSlotView view;

    protected StorageSlotController(IStorageSlot model, IStorageSlotView view) : base(model, view)
    {
        this.model = model;
        this.view = view;
    }

    public override void Initialize()
    {
        base.Initialize();
        
        disposables.Add(view.WhenSortingControlTapped.Subscribe(_ => model.Sort()));
    }
}