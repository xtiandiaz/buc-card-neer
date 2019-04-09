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
    private readonly Deck.Factory deckFactory;
    private readonly BoardView boardView;
    private readonly GameSettings settings;
    private readonly BoardCamera camera;

    private Board board;
    private Deck deck;
    private CardTile playerTile;
    private CardTile previousPlayerTile;
    private PlayerCard playerCard;
    private PlayerCardView playerCardView;
    private Dictionary<int, CardView> cardViews = new Dictionary<int, CardView>();
    private readonly ReactiveProperty<Coordinates> playerCoordinates;
    private CardTileFactory cardTileFactory;
    private IDisposable moveSubscription;

    private BoardController(
        Board.Factory boardFactory,
        Deck.Factory deckFactory,
        BoardView boardView,
        CardFactory cardFactory,
        CardTileFactory cardTileFactory,
        GameSettings settings, 
        BoardCamera camera
        )
    {
        this.boardFactory = boardFactory;
        this.deckFactory = deckFactory;
        this.boardView = boardView;
        this.cardFactory = cardFactory;
        this.cardTileFactory = cardTileFactory;
        this.settings = settings;
        this.camera = camera;
        
        playerCoordinates = new ReactiveProperty<Coordinates>(new Coordinates(0, 0));
    }

    public IPlayerCard PlayerCard => playerCard;
    public PlayerCardView PlayerCardView => playerCardView;

    public IObservable<Coordinates> PlayerCoordinates => playerCoordinates;

    public void Initialize()
    {
        board = boardFactory.Create(settings.BoardCols, settings.BoardRows);
        deck = deckFactory.Create(settings.DeckContents);
        
        foreach (var cardTile in board.Tiles)
            cardTileFactory.CreateTileView(cardTile, boardView);

        (playerCard, playerCardView) = cardFactory.Create<PlayerCard, PlayerCardView>(CardType.Player, boardView);
        playerTile = board[0, 0];
        playerTile.Card = playerCard;

        DealCard(0, 1);
        DealCard(0, -1);
        DealCard(1, 0);
        DealCard(-1, 0);

        moveSubscription = boardView.Move
            .Subscribe(direction =>
            {
                var directionVector = direction.GetVector();
                var nextCoords = new Coordinates(
                    playerCoordinates.Value.x + (int) directionVector.x, 
                    playerCoordinates.Value.y + (int) directionVector.y
                    );
                
                if (!TryMovingPlayer(nextCoords))
                    return;
                
                OnPlayerMoved(direction);
                
                if (settings.ShouldCameraFollowPlayer)
                    camera.Move(playerCoordinates.Value);
            });
    }

    public void Dispose()
    {
        moveSubscription?.Dispose();
    }

    private void DealCard(int atCoordX, int andCoordY)
    {
        var destinationTile = board[atCoordX, andCoordY];
        if (destinationTile == null || destinationTile.IsLocked)
        {
            Debug.LogWarning($"[BoardController] Cannot deal at [{atCoordX}, {andCoordY}] " +
                           $"because the Card Tile doesn't exist or is Locked (destinationTile={destinationTile})");
            return;
        }
        
        DealCard(destinationTile);
    }
    
    private void DealCard(CardTile onTile)
    {
        if (onTile.Card != null)
        {
            Debug.LogError($"[BoardController] Cannot deal on occupied Card Tile.");
            return;
        }
        
        var card = deck.Pull();
        if (card == null)
        {
            Debug.LogError($"[BoardController] The Deck is exhausted");
            return;
        }
        
        var cardKey = card.GetHashCode();
        if (!cardViews.ContainsKey(cardKey))
            cardViews[cardKey] = cardFactory.CreateView(card, boardView);
        
        onTile.Card = card;
    }

    private void Dispose(Card card)
    {
        var cardKey = card.GetHashCode();
        if (cardViews.ContainsKey(cardKey))
        {
            cardViews[cardKey].OnDispose();
            cardViews.Remove(cardKey);
        }

        deck.Push(card);
    }

    private bool TryMovingPlayer(Coordinates toCoordinates)
    {
        var nextPlayerTile = board[toCoordinates.x, toCoordinates.y];
        if (nextPlayerTile == null || nextPlayerTile == playerTile || nextPlayerTile.IsLocked)
            return false;

        playerCoordinates.Value = new Coordinates(toCoordinates.x, toCoordinates.y);

        previousPlayerTile?.Unlock();
        playerTile?.Lock();
        previousPlayerTile = playerTile;

        if (playerTile != null) 
            playerTile.Card = null;
        
        playerTile = nextPlayerTile;

        return true;
    }

    private void OnPlayerMoved(Direction towardDirection)
    {
        var cardUnderPlayer = playerTile.Card;
        if (cardUnderPlayer != null)
        {
            var cardObjectType = cardUnderPlayer.GetType();
            
            if (cardObjectType == typeof(ResourceCard))
                playerCard.Collect((ResourceCard) cardUnderPlayer);
            else if (cardObjectType == typeof(BaddieCard))
                playerCard.Perform((BaddieCard) cardUnderPlayer);
            else if (cardObjectType == typeof(AbilityCard))
                playerCard.Acquire((AbilityCard) cardUnderPlayer);
            
            Dispose(cardUnderPlayer);
        }

        // Take Stamina hit per Move
        // TODO Move somewhere else better
        playerCard.Stamina -= 1;
        
        playerTile.Card = playerCard;

        var playerValidNeighbors = playerTile.Neighbors
            .Where(n => n.Value != null && !n.Value.IsLocked && n.Value.Card == null)
            .Select(n => n.Value);
        
        foreach (var validNeighbor in playerValidNeighbors)
        {
            DealCard(validNeighbor);
        }
    }
}