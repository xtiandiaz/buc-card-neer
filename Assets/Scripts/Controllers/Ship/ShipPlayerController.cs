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
    private CardAnimationSettings cardAnimationSettings;
    
    private ShipPlayerController(IShipPlayer model, ShipPlayerView view) : base(model, view)
    {
        this.model = model;
        this.view = view;
    }

    [Inject]
    private void InjectDependencies(CardAnimationSettings cardAnimationSettings)
    {
        this.cardAnimationSettings = cardAnimationSettings;
    }
    
    public override void Initialize()
    {
        base.Initialize();
            
        disposables.Add(model.Boarding
            .Do(card =>  card.Flip(CardFace.Front))
            .Delay(TimeSpan.FromSeconds(cardAnimationSettings.BoardingDelay))
            .Where(card => (card.Type & CardType.Resource) != 0)
            .Do(card => model.Store((IResourceCard) card))
            .Subscribe());
    }
}