using System;
using UnityEngine;

public interface IBoardModel : IDisposable
{
    Vector2 Padding { get; }
    
    Vector2 CardSize { get; }
    int MaxSlotsInRow { get; }
    float SlotSpacing { get; }
    
    float Tx { get; }
    
    string SlotSortingLayerName { get; }
    string CardSortingLayerName { get; }
    int FloatingCardSortingOrder { get; }
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
    [SerializeField] private Vector2 cardSize = default;
    [SerializeField] private int maxSlotsInRow = default;
    [SerializeField] private float minSlotSpacing = default;
    [SerializeField] private float flexibleSlotSpacing = default;
    
    [Space]
    [SerializeField] private string slotSortingLayerName = default;
    [SerializeField] private string cardSortingLayerName = default;
    [SerializeField] private int floatingCardSortingOrder = default;

    private Vector2? padding;
    private float? tx, ty;

    public Vector2 Padding
    {
        get
        {
            if (padding.HasValue)
                return padding.Value;

            padding = minPadding + Tx * flexiblePadding.x * Vector2.right + Ty * flexiblePadding.y * Vector2.up;

            return padding.Value;
        }
    }

    public Vector2 CardSize => cardSize;
    public int MaxSlotsInRow => maxSlotsInRow;
    public float SlotSpacing => minSlotSpacing + flexibleSlotSpacing * Tx;
    
    public string SlotSortingLayerName => slotSortingLayerName;
    public string CardSortingLayerName => cardSortingLayerName;
    public int FloatingCardSortingOrder => floatingCardSortingOrder;
    
    public float Tx
    {
        get
        {
            if (tx.HasValue)
                return tx.Value;
            
            var refRatio = referenceAspectRatio.x / referenceAspectRatio.y;
            var wideRatio = widestAspectRatio.x / widestAspectRatio.y;
            var widthRatio = Mathf.Clamp((float) Screen.width / Screen.height, refRatio, wideRatio);
            
            tx = (widthRatio - refRatio) / (wideRatio - refRatio);

            return tx.Value;
        }
    }
    
    private float Ty
    {
        get
        {
            if (ty.HasValue)
                return ty.Value;
            
            var refRatio = referenceAspectRatio.y / referenceAspectRatio.x;
            var tallRatio = tallestAspectRatio.y / tallestAspectRatio.x;
            var heightRatio = Mathf.Clamp((float) Screen.height / Screen.width, refRatio, tallRatio);
            
            ty = (heightRatio - refRatio) / (tallRatio - refRatio);

            return ty.Value;
        }
    }

    public void Dispose()
    {
        padding = default;
        tx = default;
        ty = default;
    }
}