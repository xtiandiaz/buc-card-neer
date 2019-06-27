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
    [SerializeField] private Vector2 margins;
    [SerializeField] private Vector2 cardSize;
    [SerializeField] private Vector2 cardSpacing;
    [SerializeField] private int maxCardCountInRow;
    [SerializeField] private string slotSortingLayerName;
    [SerializeField] private string cardSortingLayerName;
    [SerializeField] private int floatingCardSortingOrder;
    
    public Vector2 Margins => margins;
    public Vector2 CardSize => cardSize;
    public Vector2 CardSpacing => cardSpacing;
    public int MaxCardCountInRow => maxCardCountInRow;
    public string SlotSortingLayerName => slotSortingLayerName;
    public string CardSortingLayerName => cardSortingLayerName;
    public int FloatingCardSortingOrder => floatingCardSortingOrder;
}