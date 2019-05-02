using UnityEngine;

[CreateAssetMenu(fileName = "BoardLayoutSettings", menuName = "Game/Settings/Board Layout Settings", order = 1)]
public class BoardLayoutSettings : ScriptableObject
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