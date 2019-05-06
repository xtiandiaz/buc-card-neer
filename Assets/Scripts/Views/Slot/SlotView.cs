using System;
using UniRx;
using UnityEngine;
using Zenject;

public interface ISlotView
{
    uint Capacity { get; }
    SlotType Type { get; }
    bool ShouldStartLocked { get; }
    ResourceType ResourceMask { get; }
    ICardArrangementSettings ArrangementSettings { get; }
    Vector3 Position { get; }
    Bounds Bounds { get; }
    
    IObservable<Unit> DraggingStart { get; }
    IObservable<Vector3> Dragging { get; }
    IObservable<Vector3> DraggingEnd { get; }

    void ToggleHighlight(bool on);
    void ToggleVisibility(bool on);
}

public class SlotView : MonoBehaviour, ISlotView
{
    [SerializeField] private SpriteRenderer faceRenderer;
    [SerializeField] private uint capacity;
    [SerializeField] private SlotType type;
    [SerializeField] private bool shouldStartLocked;
    [SerializeField] private ResourceType resourceMask;
    [SerializeField] private CardArrangementSettings arrangementSettings;
    [SerializeField] private DraggingObserver draggingObserver;

    private Color defaultFaceColor;
    private BoardLayoutSettings layoutSettings;
    private IWorldPointProvider worldPointProvider;

    public uint Capacity => capacity;
    public SlotType Type => type;
    public bool ShouldStartLocked => shouldStartLocked;
    public ResourceType ResourceMask => resourceMask;
    public ICardArrangementSettings ArrangementSettings => arrangementSettings;
    public Vector3 Position => transform.position;
    public Bounds Bounds => faceRenderer.bounds;
    
    public IObservable<Unit> DraggingStart => draggingObserver.DraggingStart;
    public IObservable<Vector3> Dragging => draggingObserver.Dragging;
    public IObservable<Vector3> DraggingEnd => draggingObserver.DraggingEnd;

    [Inject]
    private void Construct(
        BoardLayoutSettings layoutSettings, 
        IWorldPointProvider worldPointProvider
        )
    {
        this.layoutSettings = layoutSettings;
        this.worldPointProvider = worldPointProvider;
        
        defaultFaceColor = faceRenderer.color;
    }

    private void Awake()
    {
        draggingObserver.Initialize(worldPointProvider);
    }

    public void ToggleHighlight(bool on)
    {
        if (on)
        {
            faceRenderer.color = Color.green;
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