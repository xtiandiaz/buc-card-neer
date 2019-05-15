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
    
    public override void Initialize()
    {
        base.Initialize();
        
        model.PlayerSlot?.Lodge(playerCard);
            
        disposables.Add(model.WhenBoardedResource
            .Delay(TimeSpan.FromSeconds(cardAnimationSettings.BoardingDelay))
            .SelectMany(resCard => resCard.WasPurchased 
                    ? Observable.Return(resCard) 
                    : resCard.WhenPurchased.Select(_ => resCard))
            .Do(resCard => model.Store(resCard))
            .Subscribe());
    }
}