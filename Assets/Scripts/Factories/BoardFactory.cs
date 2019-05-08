using System.Linq;
using Zenject;

public interface IBoardFactory : IFactory<IBoardView, IBoard>
{
}

public class BoardFactory : IBoardFactory
{
    private readonly Board.Factory modelFactory;
    private readonly BoardController.Factory controllerFactory;
    private readonly ISeaFactory seaFactory;
    private readonly IShipFactory shipFactory;
    private readonly IDeckFactory deckFactory;

    private BoardFactory(
        Board.Factory modelFactory,
        BoardController.Factory controllerFactory,
        ISeaFactory seaFactory,
        IShipFactory shipFactory,
        IDeckFactory deckFactory
        )
    {
        this.modelFactory = modelFactory;
        this.controllerFactory = controllerFactory;
        this.seaFactory = seaFactory;
        this.shipFactory = shipFactory;
        this.deckFactory = deckFactory;
    }
    
    public IBoard Create(IBoardView fromView)
    {
        var model = modelFactory.Create(
            seaFactory.Create(fromView.Sea),
            fromView.Ships.Select(shipView => shipFactory.Create(shipView)).ToArray(),
            fromView.Decks.Select(deck => deckFactory.Create(deck)).ToArray());
        
        controllerFactory.Create(model, fromView);
        
        return model;
    }
}