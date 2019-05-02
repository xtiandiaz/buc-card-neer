using Zenject;

public class CardFoeController : CardController
{
    public class Factory : PlaceholderFactory<CardPirate, CardFoeView, CardFoeController>
    {
    }
    
    private readonly CardPirate model;
    private readonly CardFoeView view;
    
    public CardFoeController(CardPirate model, CardFoeView view) : base(model, view)
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