using UniRx;
using Zenject;

public class CardPlayerController : CardController
{
    public class Factory : PlaceholderFactory<CardPlayer, CardPlayerView, CardPlayerController>
    {
    }
    
    private readonly ICardPlayer model;
    private readonly ICardPlayerView view;
    
    public CardPlayerController(ICardPlayer model, ICardPlayerView view) : base(model, view)
    {
        this.model = model;
        this.view = view;
    }

    protected override void Initialize()
    {
        base.Initialize();
        
        disposables.Add(model.Funds.Subscribe(value => view.CoinsValue = value));
    }
}