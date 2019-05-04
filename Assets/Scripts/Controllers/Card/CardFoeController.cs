using Zenject;

public class CardFoeController : CardController
{
    public class Factory : PlaceholderFactory<CardPirate, CardPirateView, CardFoeController>
    {
    }
    
    private readonly CardPirate model;
    private readonly CardPirateView view;
    
    public CardFoeController(CardPirate model, CardPirateView view) : base(model, view)
    {
        this.model = model;
        this.view = view;
    }

    public override void Initialize()
    {
        base.Initialize();
        
        view.Value = model.Value;
    }
}