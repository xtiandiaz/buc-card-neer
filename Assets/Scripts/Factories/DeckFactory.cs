using Zenject;

public class DeckFactory : IFactory<IDeck, IDeck>
{
    private readonly DeckController.Factory controllerFactory;

    private DeckFactory(
        DeckController.Factory controllerFactory
    )
    {
        this.controllerFactory = controllerFactory;
    }
    
    public IDeck Create(IDeck fromModel)
    {
        var controller = controllerFactory.Create(fromModel);
        
        fromModel.Initialize();
        controller.Initialize();
        
        return fromModel;
    }
}