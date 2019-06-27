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
    [SerializeField] private CardType type;
    [SerializeField] private Sprite icon;
    [SerializeField] private Color color;
    
    public CardType Type => type;
    public Sprite Icon => icon;
    public Color Color => color;
}