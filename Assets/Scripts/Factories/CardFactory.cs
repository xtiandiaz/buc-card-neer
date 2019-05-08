using System;
using UnityEngine;
using Zenject;

public interface ICardFactory : IFactory<ICard, ICard>
{
}

public class CardFactory : ICardFactory
{
    private readonly DiContainer container;
    private readonly CardView.Factory viewFactory;
    private readonly CardPirateController.Factory controllerFactoryFoe;
    private readonly CardMerchantController.Factory controllerFactoryMerchant;
    private readonly CardResourceController.Factory controllerFactoryResource;
    private readonly CardPlayerController.Factory controllerFactoryPlayer;

    private CardFactory(
        DiContainer container,
        CardView.Factory viewFactory,
        CardPirateController.Factory controllerFactoryFoe,
        CardMerchantController.Factory controllerFactoryMerchant, 
        CardResourceController.Factory controllerFactoryResource, 
        CardPlayerController.Factory controllerFactoryPlayer
        )
    {
        this.container = container;
        this.viewFactory = viewFactory;
        this.controllerFactoryFoe = controllerFactoryFoe;
        this.controllerFactoryMerchant = controllerFactoryMerchant;
        this.controllerFactoryResource = controllerFactoryResource;
        this.controllerFactoryPlayer = controllerFactoryPlayer;
    }
    
    public ICard Create(ICard forModel)
    {
        container.Inject(forModel);
        
        var view = viewFactory.Create(GetViewResourcePath(forModel.Type));
        
        CreateController(forModel, view);
        
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
            
            case CardType.Player:
                
                return controllerFactoryPlayer.Create((CardPlayer) forModel, (CardPlayerView) andView);
            
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