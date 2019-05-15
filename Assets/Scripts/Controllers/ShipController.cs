using System;
using UniRx;
using UnityEngine;
using Zenject;

public interface IShipController : IInitializable, IDisposable
{
}

public class ShipController : IShipController
{
    public class Factory : PlaceholderFactory<IShip, IShipView, ShipController>
    {
    }
    
    protected readonly CompositeDisposable disposables = new CompositeDisposable();
    
    private readonly IShip model;
    private readonly IShipView view;
    private readonly ICardPlayer playerCard;
    private readonly CardAnimationSettings cardAnimationSettings;

    protected ShipController(
        IShip model, 
        IShipView view, 
        ICardPlayer playerCard, 
        CardAnimationSettings cardAnimationSettings
        )
    {
        this.model = model;
        this.view = view;
        this.playerCard = playerCard;
        this.cardAnimationSettings = cardAnimationSettings;
    }

    [Inject]
    public virtual void Initialize()
    {
        disposables.Add(model.WhenDocked.Subscribe(_ => view.Dock(model.Position)));
        disposables.Add(model.WhenSailed.Subscribe(_ => view.SetSail(model.Position)));
        
        model.PlayerSlot?.Lodge(playerCard);
            
        disposables.Add(model.WhenBoardedResource
            .Delay(TimeSpan.FromSeconds(cardAnimationSettings.BoardingDelay))
            .SelectMany(resCard => resCard.WasPurchased 
                ? Observable.Return(resCard) 
                : resCard.WhenPurchased.Select(_ => resCard))
            .Do(resCard => model.Store(resCard))
            .Subscribe());
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}