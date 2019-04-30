using Zenject;

public class CardResourceController : CardController
{
    public class Factory : PlaceholderFactory<CardResource, CardResourceView, CardResourceController>
    {
    }
    
    private readonly CardResource model;
    private readonly CardResourceView view;
    
    public CardResourceController(CardResource model, CardResourceView view) : base(model, view)
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