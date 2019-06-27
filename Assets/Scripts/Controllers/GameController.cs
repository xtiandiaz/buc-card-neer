using System;
using UniRx;
using Zenject;

public interface IGameController : IInitializable, IDisposable
{
}

public class GameController : IGameController
{
    private readonly IGameMenuView menuView;
    private readonly IAppController appController;
    private readonly ISupplyController supplyController;
    private readonly ICardHost cardHost;
    private readonly IDeck deck;
    private readonly IShip ship;
    private readonly IPlayerCard playerCard;
    private readonly ISea sea;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    public GameController(
        IGameMenuView menuView,
        IAppController appController,
        ISupplyController supplyController,
        ICardHost cardHost,
        IShip ship,
        IPlayerCard playerCard
        )
    {
        this.menuView = menuView;
        this.appController = appController;
        this.supplyController = supplyController;
        this.cardHost = cardHost;
        this.ship = ship;
        this.playerCard = playerCard;
    }
    
    public void Initialize()
    {
        disposables.Add(supplyController.Supply()
            .SelectMany(_ => cardHost.Lodge(playerCard, ship.Helm))
            .Subscribe());

        #region Menu Controls

        disposables.Add(menuView.ResetControl.OnClickAsObservable()
            .Subscribe(_ => appController.Reload()));

        #endregion
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}