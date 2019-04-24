using System;
using UnityEngine;

public class GameSettings
{
    private GameSettings()
    {
        DisplacementUnit = CardSize + CardSpacing;
    }
    
    // Gameplay
    public bool ShouldCameraFollowPlayer { get; } = true;
    public int LockedTileCount { get; } = 0; 
    public int MaxCardCountPerPlaySlot { get; } = 3;
    
    // Interaction
    public TimeSpan CardReturnDuration { get; } = TimeSpan.FromSeconds(0.5f);
    public TimeSpan CardArrangementDuration { get; } = TimeSpan.FromSeconds(0.2);
    
    // Layout
    public int BoardCols { get; } = 5;
    public int BoardRows { get; } = 5;
    public Vector2 BoardMargins { get; } = Vector2.one * 0.25f;
    public Vector2 CardSize { get; } = new Vector2(2.5f, 3.5f);
    public Vector2 CardSpacing { get; } = Vector2.zero;
    public Vector2 DisplacementUnit { get; }
    public Vector3 CardOffsetInPile { get; } = new Vector2(0.8f, 0.8f);
    public float CardThickness { get; } = 0.05f;
    public float VisibleCardCountPerRow { get; } = 4f;
    
    // Rendering
    public string SlotSortingLayerName { get; } = "Slot";
    public string CardSortingLayerName { get; } = "Card";
    public int FloatingCardSortingOrder { get; } = 50;
}