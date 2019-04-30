using Zenject;

public class CardFoeController : CardController
{
    public class Factory : PlaceholderFactory<CardFoe, CardFoeView, CardFoeController>
    {
    }
    
    private readonly CardFoe model;
    private readonly CardFoeView view;
    
    public CardFoeController(CardFoe model, CardFoeView view) : base(model, view)
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