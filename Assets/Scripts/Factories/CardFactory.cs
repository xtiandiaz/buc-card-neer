using System;

public class CardFactory : ICardFactory
{
    private readonly CardView.Factory viewFactory;
    private readonly CardFoeController.Factory controllerFactoryFoe;
    private readonly CardMerchantController.Factory controllerFactoryMerchant;
    private readonly CardResourceController.Factory controllerFactoryResource;

    private CardFactory(
        CardView.Factory viewFactory,
        CardFoeController.Factory controllerFactoryFoe,
        CardMerchantController.Factory controllerFactoryMerchant, 
        CardResourceController.Factory controllerFactoryResource
        )
    {
        this.viewFactory = viewFactory;
        this.controllerFactoryFoe = controllerFactoryFoe;
        this.controllerFactoryMerchant = controllerFactoryMerchant;
        this.controllerFactoryResource = controllerFactoryResource;
    }
    
    public ICard Create(ICard forModel)
    {
        var view = viewFactory.Create($"Prefabs/Card{forModel.Type.ToString()}");
        var controller = CreateController(forModel, view);
        
        controller.Initialize();
        
        return forModel;
    }

    private CardController CreateController(ICard forModel, ICardView andView)
    {
        switch (forModel.Type)
        {
            case CardType.Foe:
                
                return controllerFactoryFoe.Create((CardFoe) forModel, (CardFoeView) andView);
            
            case CardType.Merchant:
                
                return controllerFactoryMerchant.Create((CardMerchant) forModel, (CardMerchantView) andView);
                
            case CardType.Resource:

                return controllerFactoryResource.Create((CardResource) forModel, (CardResourceView) andView); 
            
            case CardType.Player:
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}