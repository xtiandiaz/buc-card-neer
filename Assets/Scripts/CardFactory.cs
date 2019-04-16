using System;
using Zenject;

public class CardFactory : IFactory<CardType, BoardView, CardController>
{
    private readonly GameState gameState;
    private readonly PlayerCard.Factory playerCardFactory;
    private readonly PlayerCardView.Factory playerCardViewFactory;
    private readonly ItemCard.Factory itemCardFactory;
    private readonly ItemCardView.Factory resourceCardViewFactory;
    private readonly PirateCard.Factory baddieCardFactory;
    private readonly PirateCardView.Factory pirateCardViewFactory;
    private readonly MerchantCard.Factory abilityCardFactory;
    private readonly MerchantCardView.Factory merchantCardViewFactory;
    private readonly CardController.Factory cardControllerFactory;
    private readonly BoardView boardView;

    private CardFactory(
        GameState gameState,
        PlayerCard.Factory playerCardFactory, 
        PlayerCardView.Factory playerCardViewFactory, 
        ItemCard.Factory itemCardFactory, 
        ItemCardView.Factory resourceCardViewFactory,
        PirateCard.Factory baddieCardFactory, 
        PirateCardView.Factory pirateCardViewFactory,
        MerchantCard.Factory abilityCardFactory,
        MerchantCardView.Factory merchantCardViewFactory,
        CardController.Factory cardControllerFactory, 
        BoardView boardView
        )
    {
        this.gameState = gameState;
        this.playerCardFactory = playerCardFactory;
        this.playerCardViewFactory = playerCardViewFactory;
        this.itemCardFactory = itemCardFactory;
        this.resourceCardViewFactory = resourceCardViewFactory;
        this.baddieCardFactory = baddieCardFactory;
        this.pirateCardViewFactory = pirateCardViewFactory;
        this.abilityCardFactory = abilityCardFactory;
        this.merchantCardViewFactory = merchantCardViewFactory;
        this.cardControllerFactory = cardControllerFactory;
        this.boardView = boardView;
    }

    public CardController Create(CardType type, BoardView inView) 
    {
        var model = CreateModel(type);
        return cardControllerFactory.Create(model, CreateView(model, inView));
    }
    
    public CardController Create(ICard fromModel) 
    {
        return cardControllerFactory.Create(fromModel, CreateView(fromModel, boardView));
    }
    
    public Card CreateModel(CardType forType, int withSequenceNumber = 0)
    {
        switch (forType)
        {
            case CardType.Player:
                return playerCardFactory.Create();
            case CardType.Item:
                return itemCardFactory.Create();
            case CardType.Merchant:
                return abilityCardFactory.Create();
            case CardType.Pirate:
                return baddieCardFactory.Create();
            default:
                throw new ArgumentOutOfRangeException(nameof(forType), forType, null);
        }
    }
    
    public CardView CreateView(ICard fromModel, BoardView inParentView)
    {
        var view = CreateView(fromModel);
        inParentView.ParentAsNew(view);
        return view;
    }

    private CardView CreateView(ICard fromModel)
    {
        switch (fromModel.Type)
        {
            case CardType.Player:
                return playerCardViewFactory.Create(GetResourceName(fromModel.Type));
            case CardType.Item:
                return resourceCardViewFactory.Create(GetResourceName(fromModel.Type));
            case CardType.Merchant:
                return merchantCardViewFactory.Create(GetResourceName(fromModel.Type));
            case CardType.Pirate:
                return pirateCardViewFactory.Create(GetResourceName(fromModel.Type));
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private string GetResourceName(CardType cardType)
    {
        switch (cardType)
        {
            case CardType.Player:
                return "Prefabs/Cards/Player";
            case CardType.Item:
                return "Prefabs/Cards/Item";
            case CardType.Merchant:
                return "Prefabs/Cards/Merchant";
            case CardType.Pirate:
                return "Prefabs/Cards/Pirate";
            default:
                throw new ArgumentOutOfRangeException(nameof(cardType), cardType, null);
        }
    }
}