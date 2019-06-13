using System;
using UniRx;
using Zenject;

public interface IGameController : IInitializable, IDisposable
{
}

public class GameController : IGameController
{
    private readonly IGame model;
    private readonly IGameMenuView menuView;
    private readonly IAppController appController;
    private readonly IDeckFactory deckFactory;
    private readonly ICardFactory cardFactory;
    private readonly IDeck deck;
    private readonly IPlayerCard playerCard;
    private readonly IShip ship;
    private readonly ISea sea;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    public GameController(
        IGame model, 
        IGameMenuView menuView,
        IAppController appController,
        IDeckFactory deckFactory,
        ICardFactory cardFactory,
        IDeck deck,
        IPlayerCard playerCard,
        IShip ship,
        ISea sea
        )
    {
        this.model = model;
        this.menuView = menuView;
        this.appController = appController;
        this.deckFactory = deckFactory;
        this.cardFactory = cardFactory;
        this.deck = deck;
        this.playerCard = playerCard;
        this.ship = ship;
        this.sea = sea;
    }

    public void Initialize()
    {
        deckFactory.Create(deck);
        cardFactory.Create(playerCard);
        
        ship.PlayerSlot?.Lodge(playerCard);

        disposables.Add(sea
            .Supply()
            .Subscribe());

        #region Conclusion

        disposables.Add(playerCard.WhenDestroyed.Subscribe(_ => model.End()));

        #endregion
        
        #region Menu Controls

        disposables.Add(menuView.ResetControl.OnClickAsObservable().Subscribe(_ => model.Reset()));
        disposables.Add(model.WhenReset.Subscribe(_ => appController.Reload()));

        #endregion
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}