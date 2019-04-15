using System;
using Zenject;

public class CardFactory : IFactory<CardType, BoardView, CardController>
{
    private readonly GameState gameState;
    private readonly PlayerCard.Factory playerCardFactory;
    private readonly PlayerCardView.Factory playerCardViewFactory;
    private readonly ResourceCard.Factory resourceCardFactory;
    private readonly ResourceCardView.Factory resourceCardViewFactory;
    private readonly BaddieCard.Factory baddieCardFactory;
    private readonly BaddieCardView.Factory baddieCardViewFactory;
    private readonly AbilityCard.Factory abilityCardFactory;
    private readonly AbilityCardView.Factory abilityCardViewFactory;
    private readonly CardController.Factory cardControllerFactory;
    private readonly BoardView boardView;

    private CardFactory(
        GameState gameState,
        PlayerCard.Factory playerCardFactory, 
        PlayerCardView.Factory playerCardViewFactory, 
        ResourceCard.Factory resourceCardFactory, 
        ResourceCardView.Factory resourceCardViewFactory,
        BaddieCard.Factory baddieCardFactory, 
        BaddieCardView.Factory baddieCardViewFactory,
        AbilityCard.Factory abilityCardFactory,
        AbilityCardView.Factory abilityCardViewFactory,
        CardController.Factory cardControllerFactory, 
        BoardView boardView
        )
    {
        this.gameState = gameState;
        this.playerCardFactory = playerCardFactory;
        this.playerCardViewFactory = playerCardViewFactory;
        this.resourceCardFactory = resourceCardFactory;
        this.resourceCardViewFactory = resourceCardViewFactory;
        this.baddieCardFactory = baddieCardFactory;
        this.baddieCardViewFactory = baddieCardViewFactory;
        this.abilityCardFactory = abilityCardFactory;
        this.abilityCardViewFactory = abilityCardViewFactory;
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
            case CardType.Health:
            case CardType.Stamina:
            case CardType.Defense:
                return resourceCardFactory.Create(forType);
            case CardType.Ability:
                return abilityCardFactory.Create(gameState.AbilityIndex[withSequenceNumber], withSequenceNumber);
            case CardType.Baddie:
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
                return playerCardViewFactory.Create(GetResourceName(fromModel.Type), (IPlayerCard) fromModel);
            case CardType.Health:
            case CardType.Stamina:
            case CardType.Defense:
                return resourceCardViewFactory.Create(GetResourceName(fromModel.Type), (IResourceCard)fromModel);
            case CardType.Ability:
                return abilityCardViewFactory.Create(GetResourceName(fromModel.Type), (IAbilityCard)fromModel);
            case CardType.Baddie:
                return baddieCardViewFactory.Create(GetResourceName(fromModel.Type), (IBaddieCard)fromModel);
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
            case CardType.Health:
            case CardType.Stamina:
            case CardType.Defense:
                return "Prefabs/Cards/Resource";
            case CardType.Ability:
                return "Prefabs/Cards/Ability";
            case CardType.Baddie:
                return "Prefabs/Cards/Baddie";
            default:
                throw new ArgumentOutOfRangeException(nameof(cardType), cardType, null);
        }
    }
}