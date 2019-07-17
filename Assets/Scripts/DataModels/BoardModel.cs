using System;
using UnityEngine;

public interface IBoardModel : IDisposable
{
    Vector2 Margins { get; }
    Vector2 CardSize { get; }
    Vector2 CardSpacing { get; }
    int MaxCardCountInRow { get; }
    string SlotSortingLayerName { get; }
    string CardSortingLayerName { get; }
    int FloatingCardSortingOrder { get; }
}

[CreateAssetMenu(menuName = "Model/Board")]
public class BoardModel : ScriptableObject, IBoardModel
{
    [SerializeField] private Vector2 minMargins = default;
    [SerializeField] private Vector2 flexibleMargins = default;
    [SerializeField] private Vector2 referenceAspectRatio = default;
    [SerializeField] private Vector2 widestAspectRatio = default;
    [SerializeField] private Vector2 tallestAspectRatio = default;
    
    [Space]
    [SerializeField] private Vector2 cardSize = default;
    [SerializeField] private Vector2 cardSpacing = default;
    [SerializeField] private int maxCardCountInRow = default;
    [SerializeField] private string slotSortingLayerName = default;
    [SerializeField] private string cardSortingLayerName = default;
    [SerializeField] private int floatingCardSortingOrder = default;

    private Vector2? margins = default;

    public Vector2 Margins
    {
        get
        {
            if (margins.HasValue)
                return margins.Value;
            
            var refRatio = new Vector2(
                referenceAspectRatio.x / referenceAspectRatio.y,
                referenceAspectRatio.y / referenceAspectRatio.x);

            var wideRatio = widestAspectRatio.x / widestAspectRatio.y;
            var tallRatio = tallestAspectRatio.y / tallestAspectRatio.x;

            var curRatio = new Vector2(
                Mathf.Clamp((float) Screen.width / Screen.height, refRatio.x, wideRatio),
                Mathf.Clamp((float) Screen.height / Screen.width, refRatio.y, tallRatio));
            
            var tx = (curRatio.x - refRatio.x) / (wideRatio - refRatio.x);
            var ty = (curRatio.y - refRatio.y) / (tallRatio - refRatio.y);

            margins = minMargins + tx * flexibleMargins.x * Vector2.right + ty * flexibleMargins.y * Vector2.up;

            return margins.Value;
        }
    }

    public Vector2 CardSize => cardSize;
    public Vector2 CardSpacing => cardSpacing;
    public int MaxCardCountInRow => maxCardCountInRow;
    public string SlotSortingLayerName => slotSortingLayerName;
    public string CardSortingLayerName => cardSortingLayerName;
    public int FloatingCardSortingOrder => floatingCardSortingOrder;

    public void Dispose()
    {
        margins = default;
    }
}