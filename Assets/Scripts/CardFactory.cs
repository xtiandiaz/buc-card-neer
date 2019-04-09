using System;
using Zenject;

public class CardFactory : IFactory<CardType, BoardView, Tuple<Card, CardView>>
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

    private CardFactory(
        GameState gameState,
        PlayerCard.Factory playerCardFactory, 
        PlayerCardView.Factory playerCardViewFactory, 
        ResourceCard.Factory resourceCardFactory, 
        ResourceCardView.Factory resourceCardViewFactory,
        BaddieCard.Factory baddieCardFactory, 
        BaddieCardView.Factory baddieCardViewFactory,
        AbilityCard.Factory abilityCardFactory,
        AbilityCardView.Factory abilityCardViewFactory
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
    }

    public Tuple<TModel, TView> Create<TModel, TView>(CardType type, BoardView inParentView)
        where TModel : Card
        where TView : CardView
    {
        var (model, view) = Create(type, inParentView);
        return Tuple.Create((TModel)model, (TView)view);
    }

    public Tuple<Card, CardView> Create(CardType type, BoardView inParentView)
    {
        var model = CreateModel(type);
        var view = CreateView(model, inParentView);

        return Tuple.Create(model, view);
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
    
    public CardView CreateView(Card fromModel, BoardView inParentView)
    {
        var view = CreateView(fromModel);
        inParentView.ParentAsNew(view);
        return view;
    }

    private CardView CreateView(Card fromModel)
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