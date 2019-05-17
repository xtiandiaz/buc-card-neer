using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public interface IGameController : IInitializable, IDisposable
{
}

public class GameController : IGameController
{
    private readonly IGame model;
    private readonly IGameMenuView menuView;
    private readonly IAppController appController;
    private readonly IBoardFactory boardFactory;
    private readonly ISeaFactory seaFactory;
    private readonly IShipFactory shipFactory;
    private readonly IDeckFactory deckFactory;
    private readonly ICardFactory cardFactory;
    private readonly IDeck deck;
    private readonly IPlayerCard playerCard;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    public GameController(
        IGame model, 
        IGameMenuView menuView,
        IAppController appController,
        IBoardFactory boardFactory,
        ISeaFactory seaFactory,
        IShipFactory shipFactory,
        IDeckFactory deckFactory,
        ICardFactory cardFactory,
        IDeck deck,
        IPlayerCard playerCard
        )
    {
        this.model = model;
        this.menuView = menuView;
        this.appController = appController;
        this.boardFactory = boardFactory;
        this.seaFactory = seaFactory;
        this.shipFactory = shipFactory;
        this.deckFactory = deckFactory;
        this.cardFactory = cardFactory;
        this.deck = deck;
        this.playerCard = playerCard;
    }

    public void Initialize()
    {
        deckFactory.Create(deck);
        cardFactory.Create(playerCard);

        boardFactory.Create();
        seaFactory.Create();
        shipFactory.Create();

        #region Conclusion

        disposables.Add(playerCard.WhenDestroyed.Subscribe(_ => model.End()));
        disposables.Add(model.WhenEnded.Subscribe(_ => EventSystem.current.enabled = false));

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