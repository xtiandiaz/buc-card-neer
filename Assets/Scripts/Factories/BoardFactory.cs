using System.Linq;
using Zenject;

public class BoardFactory : IFactory<IBoardView, IBoard>
{
    private readonly Board.Factory modelFactory;
    private readonly BoardController.Factory controllerFactory;
    private readonly OceanFactory oceanFactory;
    private readonly ShipFactory shipFactory;
    private readonly DeckFactory deckFactory;

    private BoardFactory(
        Board.Factory modelFactory,
        BoardController.Factory controllerFactory,
        OceanFactory oceanFactory,
        ShipFactory shipFactory,
        DeckFactory deckFactory
        )
    {
        this.modelFactory = modelFactory;
        this.controllerFactory = controllerFactory;
        this.oceanFactory = oceanFactory;
        this.shipFactory = shipFactory;
        this.deckFactory = deckFactory;
    }
    
    public IBoard Create(IBoardView withView)
    {
        withView.Initialize();
        
        var model = modelFactory.Create(
            oceanFactory.Create(withView.Ocean),
            withView.Ships.Select(sv => shipFactory.Create(sv)),
            withView.Decks.Select(d => deckFactory.Create(d)));
        
        model.Initialize();
        
        var controller = controllerFactory.Create(model, withView);
        controller.Initialize();
        
        return model;
    }
}