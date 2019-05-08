using Zenject;

public interface IDeckFactory : IFactory<IDeck, IDeck>
{
}

public class DeckFactory : IDeckFactory
{
    private readonly DiContainer container;
    private readonly DeckController.Factory controllerFactory;

    private DeckFactory(
        DiContainer container,
        DeckController.Factory controllerFactory
    )
    {
        this.container = container;
        this.controllerFactory = controllerFactory;
    }
    
    public IDeck Create(IDeck fromReferenceModel)
    {
        var model = fromReferenceModel.Clone();
        container.Inject(model);
        
        controllerFactory.Create(model);
        
        return model;
    }
}