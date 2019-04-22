using UnityEngine;
using Zenject;

public enum CardStackLayout
{
    Vertical,
    Horizontal
}

public interface ICardSlotView
{
    uint Capacity { get; }
    CardSlotType Type { get; }
    CardStackLayout Layout { get; }
    
    Transform Transform { get; }
    Vector3 HookingLocalPosition { get; }

    bool DoesContain(Vector3 worldPoint);
    void ToggleHighlight(bool on);
}

public class CardSlotView : MonoBehaviour, ICardSlotView
{
    [SerializeField] private SpriteRenderer faceRenderer;
    [SerializeField] private Transform hook;
    [SerializeField] private uint capacity;
    [SerializeField] private CardSlotType type;
    [SerializeField] private CardStackLayout layout;
    
    private Color defaultFaceColor;
    private GamePalette palette;
    private GameSettings settings;

    public uint Capacity => capacity;
    public CardSlotType Type => type;
    public CardStackLayout Layout => layout;
    public Transform Transform { get; private set; }
    public Vector3 HookingLocalPosition => transform.localPosition + hook.localPosition;
    
    [Inject]
    private void Construct(
        GamePalette palette,
        GameSettings settings
        )
    {
        Transform = transform;
        defaultFaceColor = faceRenderer.color;
        
        this.palette = palette;
        this.settings = settings;
    }

    public bool DoesContain(Vector3 worldPoint)
    {
        return faceRenderer.bounds.Contains(worldPoint);
    }

    public void ToggleHighlight(bool on)
    {
        if (on)
        {
            faceRenderer.color = palette.SlotHighlight;
            faceRenderer.sortingLayerName = settings.CardSortingLayerName;
            faceRenderer.sortingOrder = settings.FloatingCardSortingOrder - 1;
        }
        else
        {
            faceRenderer.color = defaultFaceColor;
            faceRenderer.sortingLayerName = settings.SlotSortingLayerName;
            faceRenderer.sortingOrder = 0;
        }
        
    }
}