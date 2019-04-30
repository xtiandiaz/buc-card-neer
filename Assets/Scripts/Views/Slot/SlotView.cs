using UnityEngine;
using Zenject;

public interface ISlotView
{
    uint Capacity { get; }
    SlotType Type { get; }
    Transform Transform { get; }
    Bounds Bounds { get; }
    ICardArrangement Arrangement { get; }

    void ToggleHighlight(bool on);
    void ToggleVisibility(bool on);
}

public class SlotView : MonoBehaviour, ISlotView
{
    [SerializeField] private SpriteRenderer faceRenderer;
    [SerializeField] private uint capacity;
    [SerializeField] private SlotType type;
    [SerializeField] private CardArrangement arrangement;

    private Color defaultFaceColor;
    private GamePalette palette;
    private GameSettings settings;

    public uint Capacity => capacity;
    public SlotType Type => type;
    public Transform Transform => transform;
    public Bounds Bounds => faceRenderer.bounds;
    public ICardArrangement Arrangement => arrangement;

    [Inject]
    private void Construct(
        GamePalette palette,
        GameSettings settings
        )
    {
        defaultFaceColor = faceRenderer.color;
        
        this.palette = palette;
        this.settings = settings;
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
    
    public void ToggleVisibility(bool on)
    {
        gameObject.SetActive(on);
    }
}