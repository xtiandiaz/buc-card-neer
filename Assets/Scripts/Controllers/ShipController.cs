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
    private readonly IPlayerCard playerCard;
    private readonly CardAnimationSettings cardAnimationSettings;

    protected ShipController(
        IShip model, 
        IShipView view, 
        IPlayerCard playerCard, 
        CardAnimationSettings cardAnimationSettings
        )
    {
        this.model = model;
        this.view = view;
        this.playerCard = playerCard;
        this.cardAnimationSettings = cardAnimationSettings;
    }

    [Inject]
    public void Initialize()
    {
        model.PlayerSlot?.Lodge(playerCard);

        disposables.Add(model.WhenBoardedResource
            .Delay(TimeSpan.FromSeconds(cardAnimationSettings.BoardingDelay))
            .SelectMany(resCard =>
            {
                if (resCard.CanBeCollected())
                {
                    playerCard.Collect(resCard);
                    return Observable.Return(resCard);
                }

                return resCard.WhenCanBeCollected
                    .Select(_ => resCard)
                    .Take(1)
                    .Do(playerCard.Collect);
            })
            .Do(resCard => model.Store(resCard))
            .Subscribe());
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}