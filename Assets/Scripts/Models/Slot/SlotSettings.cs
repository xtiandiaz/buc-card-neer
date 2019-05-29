using UnityEngine;

public interface ISlotSettings
{
    SlotType Type { get; }
    uint Capacity { get; }
    bool ShouldStartLocked { get; }
    SlotEntryway Entryway { get; }
    LodgingFace LodgingFace { get; }
    CardArrangement Arrangement { get; }
    Color HighlightColor { get; }
    ResourceType ResourceMask { get; }
}

[CreateAssetMenu(menuName = "Game/Settings/Slot")]
public class SlotSettings : ScriptableObject, ISlotSettings
{
    [SerializeField] private SlotType type;
    [SerializeField] [Tooltip("With 0 being infinite capacity.")] private uint capacity = 0;
    [SerializeField] private bool shouldStartLocked;
    [SerializeField] private SlotEntryway entryway = SlotEntryway.Front;
    [SerializeField] private LodgingFace lodgingFace = LodgingFace.Current;
    
    [Header("Arrangement")]
    [SerializeField] private CardArrangement arrangement;

    [Header("Highlighting")] 
    [SerializeField] private Color highlightColor = Color.green;
    
    [Header("Other")]
    [SerializeField] private ResourceType resourceMask = ResourceType.None;
    
    public SlotType Type => type;
    public uint Capacity => capacity;
    public bool ShouldStartLocked => shouldStartLocked;
    public SlotEntryway Entryway => entryway;
    public LodgingFace LodgingFace => lodgingFace;
    public CardArrangement Arrangement => arrangement;
    public Color HighlightColor => highlightColor;
    public ResourceType ResourceMask => resourceMask;
}