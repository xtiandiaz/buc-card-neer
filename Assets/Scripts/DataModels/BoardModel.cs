using UnityEngine;

public interface IBoardModel
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
    [SerializeField] private Vector2 margins = default;
    [SerializeField] private Vector2 cardSize = default;
    [SerializeField] private Vector2 cardSpacing = default;
    [SerializeField] private int maxCardCountInRow = default;
    [SerializeField] private string slotSortingLayerName = default;
    [SerializeField] private string cardSortingLayerName = default;
    [SerializeField] private int floatingCardSortingOrder = default;
    
    public Vector2 Margins => margins;
    public Vector2 CardSize => cardSize;
    public Vector2 CardSpacing => cardSpacing;
    public int MaxCardCountInRow => maxCardCountInRow;
    public string SlotSortingLayerName => slotSortingLayerName;
    public string CardSortingLayerName => cardSortingLayerName;
    public int FloatingCardSortingOrder => floatingCardSortingOrder;
}