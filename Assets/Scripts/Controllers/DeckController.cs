using Zenject;

public class DeckController
{
    public class Factory : PlaceholderFactory<Deck, DeckController>
    {
        private readonly Deck.Factory deckFactory;
        private readonly GameSettings settings;

        private Factory(
            Deck.Factory deckFactory,
            GameSettings settings
            )
        {
            this.deckFactory = deckFactory;
            this.settings = settings;
        }

        public DeckController Create()
        {
            return base.Create(deckFactory.Create(settings.DeckContents));
        }
    }
    
    private readonly Deck model;
    private readonly CardController.Factory cardControllerFactory;
    
    private DeckController(
        Deck model,
        CardController.Factory cardControllerFactory
        )
    {
        this.model = model;
        this.cardControllerFactory = cardControllerFactory;
    }
    
    public ICardController Draw()
    {
        var cardModel = model.Supply();
        
        return cardModel == null ? null : cardControllerFactory.Create(cardModel);
    }

    public void PutBack(ICard card)
    {
        model.PutBack(card);
    }
}