using Zenject;

public class CardFactory : IFactory<ICard, ICard>
{
    private readonly CardView.Factory viewFactory;
    private readonly CardController.Factory controllerFactory;

    private CardFactory(
        CardView.Factory viewFactory,
        CardController.Factory controllerFactory
        )
    {
        this.viewFactory = viewFactory;
        this.controllerFactory = controllerFactory;
    }
    
    public ICard Create(ICard forModel)
    {
        var view = viewFactory.Create($"Prefabs/Card{forModel.Type.ToString()}");
        var controller = controllerFactory.Create(forModel, view);
        
        controller.Initialize();
        
        return forModel;
    }
}