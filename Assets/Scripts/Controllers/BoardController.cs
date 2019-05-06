using System;
using Zenject;
using UniRx;

public interface IBoardController
{
    void Initialize();
}

public class BoardController : IBoardController, IDisposable
{
    public class Factory : PlaceholderFactory<IBoard, IBoardView, BoardController>
    {
    }
    
    private readonly Board model;
    private readonly BoardView view;
    private readonly ICardFactory cardFactory;
    private readonly ICardPlayer playerCard;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private BoardController(
        Board model,
        BoardView view, 
        ICardFactory cardFactory, 
        ICardPlayer playerCard
        )
    {
        this.model = model;
        this.view = view;
        this.cardFactory = cardFactory;
        this.playerCard = playerCard;
    }

    public void Initialize()
    {
        model.ShipPlayer.PlayerSlot.Take(cardFactory.Create(playerCard));

        #region Ship Docking & Sailing

        disposables.Add(model.ShipPlayer.PirateBoarding
            .Do(_ =>
            {
                model.ShipPirate.Dock(view.PirateDockingPosition);
                model.Sea.ToggleProjection(false);
            })
            .SelectMany(card => card.Destruction.Do(_ =>
            {
                model.ShipPirate.SetSail(view.PirateSailingDestination);
                model.Sea.ToggleProjection(true);
            }))
            .Subscribe());
            
        disposables.Add(model.ShipPlayer.MerchantBoarding
            .Do(_ =>
            {
                model.ShipMerchant.Dock(view.MerchantDockingPosition);
                model.Sea.ToggleProjection(false);
            })
            .SelectMany(card => card.Destruction.Do(_ =>
            {
                model.ShipMerchant.SetSail(view.MerchantDockingPosition);
                model.Sea.ToggleProjection(true);
            }))
            .Subscribe());

        #endregion
    }

    public void Dispose()
    {
        disposables?.Dispose();
    }
}