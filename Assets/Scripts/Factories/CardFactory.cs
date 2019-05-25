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
    private readonly InspectorCardController.Factory controllerFactoryInspector;
    private readonly ResourceCardController.Factory controllerFactoryResource;
    private readonly PlayerCardController.Factory controllerFactoryPlayer;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private CardFactory(
        DiContainer container,
        CardView.Factory viewFactory,
        PirateCardController.Factory controllerFactoryFoe,
        MerchantCardController.Factory controllerFactoryMerchant, 
        InspectorCardController.Factory controllerFactoryInspector, 
        ResourceCardController.Factory controllerFactoryResource, 
        PlayerCardController.Factory controllerFactoryPlayer
        )
    {
        this.container = container;
        this.viewFactory = viewFactory;
        this.controllerFactoryFoe = controllerFactoryFoe;
        this.controllerFactoryMerchant = controllerFactoryMerchant;
        this.controllerFactoryInspector = controllerFactoryInspector;
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
                
                return controllerFactoryFoe.Create((PirateCard) forModel, (PirateCardView) andView);
            
            case CardType.Merchant:
                
                return controllerFactoryMerchant.Create((MerchantCard) forModel, (MerchantCardView) andView);
            
            case CardType.Inspector:
                
                return controllerFactoryInspector.Create((IInspectorCard) forModel, (IInspectorCardView) andView);
            
            case CardType.Player:
                
                return controllerFactoryPlayer.Create((PlayerCard) forModel, (PlayerCardView) andView);
            
            default:
                
                if ((forModel.Type & CardType.Resource) != 0)
                    return controllerFactoryResource.Create((ResourceCard) forModel, (ResourceCardView) andView); 
                
                throw new ArgumentOutOfRangeException();
        }
    }

    private string GetViewResourcePath(CardType forCardType)
    {
        const string basePath = "CardViews/";
        
        switch (forCardType)
        {
            case CardType.Player:
            case CardType.Pirate:
            case CardType.Merchant:
            case CardType.Inspector:

                return $"{basePath}{forCardType}";

            default:
                
                if ((forCardType & CardType.Resource) != 0)
                    return $"{basePath}Resource"; 
                
                throw new ArgumentOutOfRangeException(nameof(forCardType), forCardType, null);
        }
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}