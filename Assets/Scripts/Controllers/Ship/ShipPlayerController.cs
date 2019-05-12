using System;
using UniRx;
using UnityEngine;
using Zenject;

public class ShipPlayerController : ShipController
{
    public class Factory : PlaceholderFactory<IShipPlayer, ShipPlayerView, ShipPlayerController>
    {
    }
    
    private readonly IShipPlayer model;
    private readonly IShipPlayerView view;
    private readonly ICardPlayer playerCard;
    private readonly CardAnimationSettings cardAnimationSettings;

    private ShipPlayerController(
        IShipPlayer model, 
        IShipPlayerView view,
        ICardPlayer playerCard, 
        CardAnimationSettings cardAnimationSettings
        ) 
        : base(model, view)
    {
        this.model = model;
        this.view = view;
        this.playerCard = playerCard;
        this.cardAnimationSettings = cardAnimationSettings;
    }
    
    protected override void Initialize()
    {
        base.Initialize();
        
        model.PlayerSlot?.Lodge(playerCard);
            
        disposables.Add(model.WhenBoarded
            .Delay(TimeSpan.FromSeconds(cardAnimationSettings.BoardingDelay))
            .Where(card => (card.Type & CardType.Resource) != 0)
            .Do(card => model.Store((IResourceCard) card))
            .Subscribe());
    }
}