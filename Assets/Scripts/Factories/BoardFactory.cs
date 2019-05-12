using Zenject;

public interface IBoardFactory : IFactory<IBoard>
{
}

public class BoardFactory : IBoardFactory
{
    private readonly Board.Factory modelFactory;
    private readonly BoardView.Factory viewFactory;
    private readonly BoardController.Factory controllerFactory;
    private readonly IShipFactory shipFactory;
    private readonly IDeckFactory deckFactory;

    private BoardFactory(
        Board.Factory modelFactory,
        BoardView.Factory viewFactory,
        BoardController.Factory controllerFactory,
        IShipFactory shipFactory,
        IDeckFactory deckFactory
        )
    {
        this.modelFactory = modelFactory;
        this.viewFactory = viewFactory;
        this.controllerFactory = controllerFactory;
        this.shipFactory = shipFactory;
        this.deckFactory = deckFactory;
    }
    
    public IBoard Create()
    {
        var model = modelFactory.Create();
        var view = viewFactory.Create();
        
        controllerFactory.Create(model, view);
        
        return model;
    }
}