using System.Linq;
using Zenject;

public interface IBoardFactory : IFactory<IBoard>
{
}

public class BoardFactory : IBoardFactory
{
    private readonly IStage stage;
    private readonly IBoardModel model;
    private readonly ISlotModel supplySlotModel;
    private readonly Board.Factory instanceFactory;
    private readonly Sea.Factory seaFactory;
    private readonly IShipFactory shipFactory;
    private readonly ISlotFactory slotFactory;
    private readonly IBoardView boardView;
    private readonly IBoardLayout boardLayout;

    private BoardFactory(
        IStage stage,
        IBoardModel model,
        ISlotModel supplySlotModel,
        Board.Factory instanceFactory,
        Sea.Factory seaFactory,
        IShipFactory shipFactory,
        ISlotFactory slotFactory,
        IBoardView boardView,
        IBoardLayout boardLayout
    )
    {
        this.stage = stage;
        this.model = model;
        this.supplySlotModel = supplySlotModel;
        this.instanceFactory = instanceFactory;
        this.seaFactory = seaFactory;
        this.shipFactory = shipFactory;
        this.slotFactory = slotFactory;
        this.boardView = boardView;
        this.boardLayout = boardLayout;
    }
    
    public IBoard Create()
    {
        var supplySlots = Enumerable.Range(0, stage.SupplySize)
            .Select(i => slotFactory.Create(supplySlotModel));

        var sea = seaFactory.Create(supplySlots, boardView.Sea);
        var ship = shipFactory.Create();
        
        return instanceFactory.Create(sea, ship, model);
    }
}