using Zenject;

public class DeckFactory : IFactory<DeckContents, DeckController>
{
    private Deck.Factory deckFactory;
    private DeckController.Factory deckControllerFactory;

    private DeckFactory(
        Deck.Factory deckFactory, 
        DeckController.Factory deckControllerFactory
        )
    {
        this.deckFactory = deckFactory;
        this.deckControllerFactory = deckControllerFactory;
    }
    
    public DeckController Create(DeckContents withContents)
    {
        return deckControllerFactory.Create(deckFactory.Create(withContents));
    }
}