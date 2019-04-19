using UniRx;
using UnityEngine;
using Zenject;

public interface ICardSlotView
{
    uint Capacity { get; }
    CardSlotType Type { get; }
    Transform Transform { get; }

    bool DoesContain(Vector3 worldPoint);
}

public class CardSlotView : MonoBehaviour, ICardSlotView
{
    [SerializeField] private SpriteRenderer faceRenderer;
    [SerializeField] private SpriteRenderer forbiddenIconRenderer;
    [SerializeField] private uint capacity;
    [SerializeField] private CardSlotType type;
    
    private GameSettings settings;

    public uint Capacity => capacity;
    public CardSlotType Type => type;
    public Transform Transform { get; private set; }
    
    [Inject]
    private void Construct(
        GameSettings settings
        )
    {
        Transform = transform;
        
        this.settings = settings;
    }

    public bool DoesContain(Vector3 worldPoint)
    {
        return faceRenderer.bounds.Contains(worldPoint);
    }
}