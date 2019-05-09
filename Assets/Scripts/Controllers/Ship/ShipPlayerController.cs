using System;
using UniRx;
using Zenject;

public class ShipPlayerController : ShipController
{
    public class Factory : PlaceholderFactory<IShipPlayer, ShipPlayerView, ShipPlayerController>
    {
    }
    
    private readonly IShipPlayer model;
    private readonly ShipPlayerView view;
    [Inject] private CardAnimationSettings cardAnimationSettings;
    
    private ShipPlayerController(IShipPlayer model, ShipPlayerView view) : base(model, view)
    {
        this.model = model;
        this.view = view;
    }
    
    protected override void Initialize()
    {
        base.Initialize();
            
        disposables.Add(model.WhenBoarded
            .Delay(TimeSpan.FromSeconds(cardAnimationSettings.BoardingDelay))
            .Where(card => (card.Type & CardType.Resource) != 0)
            .Do(card => model.Store((IResourceCard) card))
            .Subscribe());
    }
}