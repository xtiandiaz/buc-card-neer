using System;
using UnityEngine;

public class GameSettings
{
    private GameSettings()
    {
        DisplacementUnit = CardSize + CardSpacing;
        MoveDurationInSeconds = (float) MoveDuration.TotalSeconds;
    }
    
    // Gameplay
    public bool ShouldCameraFollowPlayer { get; } = true;
    public int LockedTileCount { get; } = 0; 
    
    // Layout
    public int BoardCols { get; } = 5;
    public int BoardRows { get; } = 5;
    public Vector2 CardSize { get; } = new Vector2(2.5f, 3.5f);
    public Vector2 CardSpacing { get; } = Vector2.one * 0.25f;
    public Vector2 DisplacementUnit { get; }
    public float VisibleCardCountPerRow { get; } = 3.25f;
    public TimeSpan MoveDuration { get; } = TimeSpan.FromSeconds(0.75);
    public float MoveDurationInSeconds { get; }
    
    // Deck
    public DeckContents DeckContents { get; } = new DeckContents(
        new CardClass(CardType.Health, 9),
        new CardClass(CardType.Stamina, 13),
        new CardClass(CardType.Health, 10),
        new CardClass(CardType.Defense, 7),
        new CardClass(CardType.Ability, 5), 
        new CardClass(CardType.Baddie, 19) 
        );
    
    // Player
    public int StartPlayerHealth { get; } = 10;
    public int StartPlayerStamina { get; } = 10;
    public int StartPlayerDefense { get; } = 10;
    public int MaxPlayerHealth { get; } = 20;
    public int MaxPlayerStamina { get; } = 20;
    public int MaxPlayerDefense { get; } = 20;
    
    // Rendering
    public string CardDefaultSortingLayerName { get; } = "Card";
    public string CardTextDefaultSortingLayerName { get; } = "Card Text";
    public string PlayerDefaultSortingLayerName { get; } = "Player";
    public string PlayerTextDefaultSortingLayerName { get; } = "Player Text";
    public string CardFirstOverlaySortingLayerName { get; } = "Card Overlay 1";
    public string CardTextFirstOverlaySortingLayerName { get; } = "Card Text Overlay 1";
    public string CardSecondOverlaySortingLayerName { get; } = "Card Overlay 2";
    public string CardTextSecondOverlaySortingLayerName { get; } = "Card Text Overlay 2";
}