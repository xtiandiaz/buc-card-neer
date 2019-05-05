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
        var view = viewFactory.Create(GetViewResourcePath(forModel.Type));
        var controller = CreateController(forModel, view);
        
        controller.Initialize();
        
        return forModel;
    }

    private CardController CreateController(ICard forModel, ICardView andView)
    {
        switch (forModel.Type)
        {
            case CardType.Pirate:
                
                return controllerFactoryFoe.Create((CardPirate) forModel, (CardPirateView) andView);
            
            case CardType.Merchant:
                
                return controllerFactoryMerchant.Create((CardMerchant) forModel, (CardMerchantView) andView);
            
            default:
                
                if ((forModel.Type & CardType.Resource) != 0)
                    return controllerFactoryResource.Create((CardResource) forModel, (CardResourceView) andView); 
                
                throw new ArgumentOutOfRangeException();
        }
    }

    private string GetViewResourcePath(CardType forCardType)
    {
        const string basePath = "Prefabs/Card";
        
        switch (forCardType)
        {
            case CardType.Player:
            case CardType.Pirate:
            case CardType.Merchant:

                return basePath + forCardType;

            default:
                
                if ((forCardType & CardType.Resource) != 0)
                    return basePath + "Resource"; 
                
                throw new ArgumentOutOfRangeException(nameof(forCardType), forCardType, null);
        }
    }
}