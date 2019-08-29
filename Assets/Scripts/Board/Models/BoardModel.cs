using UnityEngine;

public interface IBoardModel
{
    Vector2 ReferenceAspectRatio { get; }
    Vector2 WidestAspectRatio { get; }
    Vector2 TallestAspectRatio { get; }
    
    Vector2 MinPadding { get; }
    Vector2 FlexiblePadding { get; }

    float MinSlotSpacing { get; }
    float FlexibleSlotSpacing { get; }
    
    int MaxSupplySlotCount { get; }
    
    int CardCountPerSupplySlot { get; }
    int MaxCardsInSupply { get; }
    
    string SlotSortingLayerName { get; }
    string CardSortingLayerName { get; }
    int FloatingCardSortingOrder { get; }
    float FloatingCardDepth { get; }
}

[CreateAssetMenu(menuName = "Model/Board")]
public class BoardModel : ScriptableObject, IBoardModel
{
    [SerializeField] private Vector2 minPadding = default;
    [SerializeField] private Vector2 flexiblePadding = default;
    [SerializeField] private Vector2 referenceAspectRatio = default;
    [SerializeField] private Vector2 widestAspectRatio = default;
    [SerializeField] private Vector2 tallestAspectRatio = default;
    
    [Space]
    [SerializeField] private int supplySlotCount = default;
    [SerializeField] private float minSlotSpacing = default;
    [SerializeField] private float flexibleSlotSpacing = default;
    
    [Space]
    [SerializeField] private int cardCountPerSupplySlot = default;

    [Space]
    [SerializeField] private string slotSortingLayerName = default;
    [SerializeField] private string cardSortingLayerName = default;
    [SerializeField] private int floatingCardSortingOrder = default;
    [SerializeField] private float floatingCardDepth = default;

    public Vector2 ReferenceAspectRatio => referenceAspectRatio;
    public Vector2 WidestAspectRatio => widestAspectRatio;
    public Vector2 TallestAspectRatio =>  tallestAspectRatio;

    public Vector2 MinPadding => minPadding;
    public Vector2 FlexiblePadding => flexiblePadding;

    public float MinSlotSpacing => minSlotSpacing;
    public float FlexibleSlotSpacing => flexibleSlotSpacing;

    public int MaxSupplySlotCount => supplySlotCount;

    public int CardCountPerSupplySlot => cardCountPerSupplySlot;
    public int MaxCardsInSupply => cardCountPerSupplySlot * supplySlotCount;

    public string SlotSortingLayerName => slotSortingLayerName;
    public string CardSortingLayerName => cardSortingLayerName;
    public int FloatingCardSortingOrder => floatingCardSortingOrder;
    public float FloatingCardDepth => floatingCardDepth;
}