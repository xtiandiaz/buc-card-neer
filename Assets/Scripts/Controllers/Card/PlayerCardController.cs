using UniRx;
using Zenject;

public class PlayerCardController : CardController
{
    public class Factory : PlaceholderFactory<PlayerCard, CardPlayerView, PlayerCardController>
    {
    }
    
    private readonly IPlayerCard model;
    private readonly ICardPlayerView view;
    
    public PlayerCardController(IPlayerCard model, ICardPlayerView view) : base(model, view)
    {
        this.model = model;
        this.view = view;
    }

    public override void Initialize()
    {
        base.Initialize();
        
        disposables.Add(model.Funds.Subscribe(value => view.CoinsValue = value));
    }
}