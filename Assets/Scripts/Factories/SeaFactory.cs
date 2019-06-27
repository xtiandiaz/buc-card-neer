using System.Collections.Generic;
using System.Linq;
using UniRx;
using Zenject;
using Object = UnityEngine.Object;

public interface ISeaFactory : IFactory<ISea>
{
}

public class SeaFactory : ISeaFactory
{
    private readonly Sea.Factory seaFactory;
    private readonly IBoardView boardView;
    private readonly ISlotFactory slotFactory;
    private readonly List<SlotModel> supplySlotModels;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private SeaFactory(
        Sea.Factory seaFactory,
        IBoardView boardView,
        ISlotFactory slotFactory,
        List<SlotModel> supplySlotModels
        )
    {
        this.seaFactory = seaFactory;
        this.boardView = boardView;
        this.slotFactory = slotFactory;
        this.supplySlotModels = supplySlotModels;
    }

    public ISea Create()
    {
        var view = boardView.Sea;

        var supplySlots = supplySlotModels
            .Select((slotModel, i) => slotFactory.Create(Object.Instantiate(slotModel), view.Slots[i]));

        return seaFactory.Create(supplySlots, view);
    }
}