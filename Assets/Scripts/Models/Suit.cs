using UnityEngine;

public interface ISuit
{
    Sprite Icon { get; }
    Color Color { get; }
    ResourceType ResourceType { get; }
}

[CreateAssetMenu(fileName = "Suit", menuName = "Game/Suit", order = 1)]
public class Suit : ScriptableObject, ISuit
{
    [SerializeField] private ResourceType resourceType;
    [SerializeField] private Sprite icon;
    [SerializeField] private Color color;
    
    public Sprite Icon => icon;
    public Color Color => color;
    public ResourceType ResourceType => resourceType;
}