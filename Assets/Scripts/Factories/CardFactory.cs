using System;
using UniRx;
using UnityEngine;
using Zenject;

public interface ICardFactory : IFactory<ICard, ICard>, IDisposable
{
    ICard Create(ICard forModel);
}

public class CardFactory : ICardFactory
{
    private readonly DiContainer container;
    private readonly CardView.Factory viewFactory;
    private readonly PirateCardController.Factory controllerFactoryFoe;
    private readonly MerchantCardController.Factory controllerFactoryMerchant;
    private readonly ResourceCardController.Factory controllerFactoryResource;
    private readonly PlayerCardController.Factory controllerFactoryPlayer;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private CardFactory(
        DiContainer container,
        CardView.Factory viewFactory,
        PirateCardController.Factory controllerFactoryFoe,
        MerchantCardController.Factory controllerFactoryMerchant, 
        ResourceCardController.Factory controllerFactoryResource, 
        PlayerCardController.Factory controllerFactoryPlayer
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

        disposables.Add(CreateController(forModel, view));
        
        return forModel;
    }

    private CardController CreateController(ICard forModel, ICardView andView)
    {
        switch (forModel.Type)
        {
            case CardType.Pirate:
                
                return controllerFactoryFoe.Create((PirateCard) forModel, (CardPirateView) andView);
            
            case CardType.Merchant:
                
                return controllerFactoryMerchant.Create((MerchantCard) forModel, (CardMerchantView) andView);
            
            case CardType.Player:
                
                return controllerFactoryPlayer.Create((PlayerCard) forModel, (CardPlayerView) andView);
            
            default:
                
                if ((forModel.Type & CardType.Resource) != 0)
                    return controllerFactoryResource.Create((ResourceCard) forModel, (CardResourceView) andView); 
                
                throw new ArgumentOutOfRangeException();
        }
    }

    private string GetViewResourcePath(CardType forCardType)
    {
        const string basePath = "CardViews/Card";
        
        switch (forCardType)
        {
            case CardType.Player:
            case CardType.Pirate:
            case CardType.Merchant:

                return $"{basePath}{forCardType}";

            default:
                
                if ((forCardType & CardType.Resource) != 0)
                    return $"{basePath}Resource"; 
                
                throw new ArgumentOutOfRangeException(nameof(forCardType), forCardType, null);
        }
    }

    public void Dispose()
    {
        disposables?.Dispose();
    }
}