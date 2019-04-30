public class DeckFactory : IDeckFactory
{
    private readonly DeckController.Factory controllerFactory;

    private DeckFactory(
        DeckController.Factory controllerFactory
    )
    {
        this.controllerFactory = controllerFactory;
    }
    
    public IDeck Create(IDeck forModel)
    {
        var controller = controllerFactory.Create(forModel);
        
        forModel.Initialize();
        controller.Initialize();
        
        return forModel;
    }
}