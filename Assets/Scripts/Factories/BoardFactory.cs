using System.Linq;
using Zenject;

public interface IBoardFactory : IFactory<IBoard>
{
}

public class BoardFactory : IBoardFactory
{
    private readonly IBoardView boardView;
    private readonly Board.Factory modelFactory;
    private readonly BoardController.Factory controllerFactory;
    private readonly ISeaFactory seaFactory;
    private readonly IShipFactory shipFactory;
    private readonly IDeckFactory deckFactory;

    private BoardFactory(
        IBoardView boardView,
        Board.Factory modelFactory,
        BoardController.Factory controllerFactory,
        ISeaFactory seaFactory,
        IShipFactory shipFactory,
        IDeckFactory deckFactory
        )
    {
        this.boardView = boardView;
        this.modelFactory = modelFactory;
        this.controllerFactory = controllerFactory;
        this.seaFactory = seaFactory;
        this.shipFactory = shipFactory;
        this.deckFactory = deckFactory;
    }
    
    public IBoard Create()
    {
        var model = modelFactory.Create(
            seaFactory.Create(boardView.Sea),
            boardView.Ships.Select(shipView => shipFactory.Create(shipView)).ToArray(),
            boardView.Decks.Select(deck => deckFactory.Create(deck)).ToArray());
        
        controllerFactory.Create(model, boardView);
        
        return model;
    }
}