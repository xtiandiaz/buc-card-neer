using System;
using UniRx;
using UnityEngine;
using Zenject;

public interface ISlotView
{
    uint Capacity { get; }
    SlotType Type { get; }
    bool ShouldStartLocked { get; }
    CardFace SlottingFace { get; }
    ResourceType ResourceMask { get; }
    ICardArrangement CardArrangement { get; }
    Vector3 Position { get; }
    Bounds Bounds { get; }
    
    IObservable<Unit> WhenDraggingStarted { get; }
    IObservable<Vector3> WhenDragged { get; }
    IObservable<Vector3> WhenDraggingStopped { get; }

    void ToggleHighlight(bool on);
    void ToggleVisibility(bool on);
}

public class SlotView : MonoBehaviour, ISlotView
{
    [SerializeField] private SpriteRenderer faceRenderer;
    [SerializeField] private uint capacity;
    [SerializeField] private SlotType type;
    [SerializeField] private bool shouldStartLocked;
    [SerializeField] private CardFace slottingFace = CardFace.Front;
    [SerializeField] private ResourceType resourceMask;
    [SerializeField] private CardArrangement arrangement;
    [SerializeField] private DraggingObserver draggingObserver;

    private Color defaultFaceColor;
    private BoardLayoutSettings layoutSettings;
    private IWorldPointProvider worldPointProvider;

    public uint Capacity => capacity;
    public SlotType Type => type;
    public bool ShouldStartLocked => shouldStartLocked;
    public CardFace SlottingFace => slottingFace;
    public ResourceType ResourceMask => resourceMask;
    public ICardArrangement CardArrangement => arrangement;
    public Vector3 Position => transform.position;
    public Bounds Bounds => faceRenderer.bounds;
    
    public IObservable<Unit> WhenDraggingStarted => draggingObserver.DraggingStart;
    public IObservable<Vector3> WhenDragged => draggingObserver.Dragging;
    public IObservable<Vector3> WhenDraggingStopped => draggingObserver.DraggingEnd;

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