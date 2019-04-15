using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using UniRx;

public struct Coordinates
{
    public int x;
    public int y;

    public Coordinates(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public class BoardController : IInitializable, IDisposable
{
    private readonly CardFactory cardFactory;
    private readonly Board.Factory boardFactory;
    private readonly DeckFactory deckFactory;
    private readonly BoardView boardView;
    private readonly GameSettings settings;
    private readonly CardSlotFactory cardSlotFactory;
    private readonly CompositeDisposable perGameDisposables = new CompositeDisposable();

    private Board board;
    private DeckController deck;
    private Dictionary<int, CardView> cardViews = new Dictionary<int, CardView>();
    private IDisposable moveSubscription;

    private List<CardSlotController> playSlots;
    private List<CardSlotController> stashSlots;
    private CardSlotController playerSlot;

    private BoardController(
        Board.Factory boardFactory,
        DeckFactory deckFactory,
        BoardView boardView,
        CardFactory cardFactory,
        CardSlotFactory cardSlotFactory,
        GameSettings settings
        )
    {
        this.boardFactory = boardFactory;
        this.deckFactory = deckFactory;
        this.boardView = boardView;
        this.cardFactory = cardFactory;
        this.cardSlotFactory = cardSlotFactory;
        this.settings = settings;
    }

    public void Initialize()
    {
        board = boardFactory.Create(settings.BoardCols, settings.BoardRows);
        deck = deckFactory.Create(settings.DeckContents);

        playSlots = boardView.PlaySlots.Select(slotView => cardSlotFactory.Create(slotView)).ToList();
        stashSlots = boardView.StashSlots.Select(slotView => cardSlotFactory.Create(slotView)).ToList();
        playerSlot = cardSlotFactory.Create(boardView.PlayerSlot);

        perGameDisposables.Add(
            playSlots.Select(s => s.Emptied.Select(_ => s)).Merge()
                .Subscribe(s => Deal(s, s.Model.Capacity)));
    }

    public void Dispose()
    {
        perGameDisposables.Dispose();
    }

    public void Deal(CardSlotController onSlot, int count)
    {
        for (var i = 0; i < count; i++)
        {
            if (!Deal(onSlot))
                break;
        }
    }
    
    public bool Deal(CardSlotController onSlot)
    {
        if (!onSlot.DoesAcceptNewCards)
            return false;

        var cardController = deck.Dequeue();
        if (cardController == null)
            return false;

        onSlot.Take(cardController);

        return true;
    }
}