using System;
using UniRx;
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
    private readonly IDeckFactory deckFactory;
    private readonly ICardFactory cardFactory;
    private readonly IShipFactory shipFactory;
    private readonly IDeck deck;
    private readonly IPlayerCard playerCard;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    public GameController(
        IGame model, 
        IGameMenuView menuView,
        IAppController appController,
        IDeckFactory deckFactory,
        ICardFactory cardFactory,
        IShipFactory shipFactory,
        IDeck deck,
        IPlayerCard playerCard
        )
    {
        this.model = model;
        this.menuView = menuView;
        this.appController = appController;
        this.deckFactory = deckFactory;
        this.cardFactory = cardFactory;
        this.shipFactory = shipFactory;
        this.deck = deck;
        this.playerCard = playerCard;
    }

    public void Initialize()
    {
        deckFactory.Create(deck);
        cardFactory.Create(playerCard);
        shipFactory.Create();

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