using UnityEngine;

public interface ISlotSettings
{
    SlotType Type { get; }
    uint Capacity { get; }
    bool ShouldStartLocked { get; }
    SlotEntryway Entryway { get; }
    CardArrangement Arrangement { get; }
    bool DoesSelfArrangeOnLodging { get; }
    bool DoesSelfArrangeOnReleasing { get; }
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
    
    [Header("Arrangement")]
    [SerializeField] private CardArrangement arrangement;
    [SerializeField] private bool doesSelfArrangeOnLodging = true;
    [SerializeField] private bool doesSelfArrangeOnReleasing = true;

    [Header("Highlighting")] 
    [SerializeField] private Color highlightColor = Color.green;
    
    [Header("Other")]
    [SerializeField] private ResourceType resourceMask = ResourceType.None;
    
    public SlotType Type => type;
    public uint Capacity => capacity;
    public bool ShouldStartLocked => shouldStartLocked;
    public SlotEntryway Entryway => entryway;
    public CardArrangement Arrangement => arrangement;
    public bool DoesSelfArrangeOnLodging => doesSelfArrangeOnLodging;
    public bool DoesSelfArrangeOnReleasing => doesSelfArrangeOnReleasing;
    public Color HighlightColor => highlightColor;
    public ResourceType ResourceMask => resourceMask;
}