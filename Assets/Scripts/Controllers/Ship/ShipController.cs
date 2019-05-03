using System;
using UniRx;
using UnityEngine;
using Zenject;

public interface IShipController
{
    void Initialize();
}

public class ShipController : IShipController, IDisposable
{
    public class Factory : PlaceholderFactory<IShip, IShipView, ShipController>
    {
    }
    
    private readonly IShip model;
    private readonly IShipView view;
    private readonly CardAnimationSettings cardAnimationSettings;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private ShipController(
        IShip model, 
        IShipView view, 
        CardAnimationSettings cardAnimationSettings)
    {
        this.model = model;
        this.view = view;
        this.cardAnimationSettings = cardAnimationSettings;
    }

    public void Initialize()
    {
        var viewHidingPosition = Vector3.up * (view.ViewportHeight + view.Height * 0.5f);
        
        disposables.Add(model.Docked.Subscribe(atPosition => view.Dock(atPosition)));
        disposables.Add(model.Sailed.Subscribe(_ => view.SetSail(viewHidingPosition)));
        
        disposables.Add(model.Boarded
            .Do(card =>  card.Flip(CardFace.Front))
            .Delay(TimeSpan.FromSeconds(cardAnimationSettings.BoardingDelay))
            .Where(card => card.Type == CardType.Resource)
            .Do(card => model.Store((IResourceCard) card))
            .Subscribe());
    }

    public void Dispose()
    {
        disposables?.Dispose();
    }
}