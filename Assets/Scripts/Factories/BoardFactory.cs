using System.Linq;

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
        fromView.Initialize();
        
        var model = modelFactory.Create(
            seaFactory.Create(fromView.Sea),
            fromView.Ships.Select(sv => shipFactory.Create(sv)).ToArray(),
            fromView.Decks.Select(d => deckFactory.Create(d)).ToArray());
        
        var controller = controllerFactory.Create(model, fromView);
        
        controller.Initialize();
        
        return model;
    }
}