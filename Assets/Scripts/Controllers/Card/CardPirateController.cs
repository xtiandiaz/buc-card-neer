using Zenject;

public class CardPirateController : CardController
{
    public class Factory : PlaceholderFactory<CardPirate, CardPirateView, CardPirateController>
    {
    }
    
    private readonly CardPirate model;
    private readonly CardPirateView view;
    
    public CardPirateController(CardPirate model, CardPirateView view) : base(model, view)
    {
        this.model = model;
        this.view = view;
    }
}