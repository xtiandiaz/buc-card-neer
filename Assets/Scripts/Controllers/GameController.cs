using System;
using UniRx;
using UnityEngine;
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
    private readonly ICardPlayer cardPlayer;
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
        ICardPlayer cardPlayer
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
        this.cardPlayer = cardPlayer;
    }

    public void Initialize()
    {
        deckFactory.Create(deck);
        cardFactory.Create(cardPlayer);

        boardFactory.Create();
        seaFactory.Create();
        shipFactory.Create(ShipType.Player);

        disposables.Add(menuView.ResetControl.OnClickAsObservable().Subscribe(_ => model.Reset()));
        disposables.Add(model.WhenReset.Subscribe(_ => appController.Reload()));
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}