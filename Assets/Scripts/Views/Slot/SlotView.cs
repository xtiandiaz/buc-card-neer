using UnityEngine;
using Zenject;

public interface ISlotView
{
    uint Capacity { get; }
    SlotType Type { get; }
    ResourceType ResourceMask { get; }
    ICardArrangement Arrangement { get; }
    Vector3 Position { get; }
    Bounds Bounds { get; }

    void ToggleHighlight(bool on);
    void ToggleVisibility(bool on);
}

public class SlotView : MonoBehaviour, ISlotView
{
    [SerializeField] private SpriteRenderer faceRenderer;
    [SerializeField] private uint capacity;
    [SerializeField] private SlotType type;
    [SerializeField] private ResourceType resourceMask;
    [SerializeField] private CardArrangement arrangement;

    private Color defaultFaceColor;
    private BoardLayoutSettings layoutSettings;

    public uint Capacity => capacity;
    public SlotType Type => type;
    public ResourceType ResourceMask => resourceMask;
    public ICardArrangement Arrangement => arrangement;
    public Vector3 Position => transform.position;
    public Bounds Bounds => faceRenderer.bounds;

    [Inject]
    private void Construct(
        BoardLayoutSettings layoutSettings
        )
    {
        this.layoutSettings = layoutSettings;
        
        defaultFaceColor = faceRenderer.color;
    }

    public void ToggleHighlight(bool on)
    {
        if (on)
        {
            faceRenderer.color = Color.cyan;
            faceRenderer.sortingLayerName = layoutSettings.CardSortingLayerName;
            faceRenderer.sortingOrder = layoutSettings.FloatingCardSortingOrder - 1;
        }
        else
        {
            faceRenderer.color = defaultFaceColor;
            faceRenderer.sortingLayerName = layoutSettings.SlotSortingLayerName;
            faceRenderer.sortingOrder = 0;
        }
    }
    
    public void ToggleVisibility(bool on)
    {
        gameObject.SetActive(on);
    }
}