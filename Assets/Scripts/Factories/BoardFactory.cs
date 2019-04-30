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
    
    public IBoard Create(IBoardView forModel)
    {
        forModel.Initialize();
        
        var model = modelFactory.Create(
            seaFactory.Create(forModel.Sea),
            forModel.Ships.Select(sv => shipFactory.Create(sv)).ToArray(),
            forModel.Decks.Select(d => deckFactory.Create(d)).ToArray());
        
        model.Initialize();
        
        var controller = controllerFactory.Create(model, forModel);
        controller.Initialize();
        
        return model;
    }
}