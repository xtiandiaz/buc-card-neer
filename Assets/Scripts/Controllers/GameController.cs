using System;
using UniRx;
using UnityEngine;
using Zenject;

public class GameController : IInitializable, IDisposable
{
    private readonly IBoardFactory boardFactory;
    private readonly ISeaFactory seaFactory;
    private readonly IShipFactory shipFactory;
    private readonly IDeckFactory deckFactory;
    private readonly ICardFactory cardFactory;
    private readonly IDeck deck;
    private readonly ICardPlayer cardPlayer;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    public GameController(
        IBoardFactory boardFactory,
        ISeaFactory seaFactory,
        IShipFactory shipFactory,
        IDeckFactory deckFactory,
        ICardFactory cardFactory,
        IDeck deck,
        ICardPlayer cardPlayer
        )
    {
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
        Application.targetFrameRate = 50;
        
        deckFactory.Create(deck);
        cardFactory.Create(cardPlayer);
        
        boardFactory.Create();
        seaFactory.Create();
        shipFactory.Create(ShipType.Player);
    }

    public void Dispose()
    {
        disposables?.Dispose();
    }
}