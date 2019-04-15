using Zenject;

public class DeckController
{
    private readonly Deck model;
    private readonly CardFactory cardFactory;

    public class Factory : PlaceholderFactory<Deck, DeckController>
    {
    }

    private DeckController(
        Deck model,
        CardFactory cardFactory
        )
    {
        this.model = model;
        this.cardFactory = cardFactory;
    }

    public void Queue(CardController cardController)
    {
        model.Queue(cardController.Model);
        
        cardController.Destroy();
    }

    public CardController Dequeue()
    {
        var cardModel = model.Dequeue();
        
        return cardModel == null ? null : cardFactory.Create(cardModel);
    }
}