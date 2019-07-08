using UnityEngine;

public interface ISuitModel
{
    CardType Type { get; }
    Sprite Icon { get; }
    Color Color { get; }
}

[CreateAssetMenu(menuName = "Model/Suit")]
public class SuitModel : ScriptableObject, ISuitModel
{
    [SerializeField] private CardType type = default;
    [SerializeField] private Sprite icon = default;
    [SerializeField] private Color color = default;
    
    public CardType Type => type;
    public Sprite Icon => icon;
    public Color Color => color;
}