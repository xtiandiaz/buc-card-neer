using UniRx;
using UnityEngine;
using Zenject;

public interface ICardSlotView
{
    int InitialCapacity { get; }
    Vector3 LocalPosition { get; }
}

public class CardSlotView : MonoBehaviour, ICardSlotView
{
    public class Factory : PlaceholderFactory<ICardSlot, ICardSlotView>
    {
    }

    [SerializeField] private SpriteRenderer forbiddenIconRenderer;
    [SerializeField] private int initialCapacity;
    
    private GameSettings settings;

    public int InitialCapacity => initialCapacity;
    public Vector3 LocalPosition => transform.localPosition;
    
    [Inject]
    private void Construct(GameSettings settings)
    {
        this.settings = settings;
    }
}