using UnityEngine;

public enum SlotEntryway
{
    Front, 
    Rear
}

public interface ISlotModel
{
    SlotType Type { get; }
    uint Capacity { get; }
    SlotEntryway Entryway { get; }
    bool ShouldStartLocked { get; }
    int Order { get; }
    
    Color HighlightColor { get; }
    
    CardArrangementModel Arrangement { get; }
}

[CreateAssetMenu(menuName = "Model/Slot")]
public class SlotModel : ScriptableObject, ISlotModel
{
    [SerializeField] private SlotType type;
    [SerializeField] private uint capacity;
    [SerializeField] private SlotEntryway entryway = SlotEntryway.Front;
    [SerializeField] private bool shouldStartLocked;
    [SerializeField] private int order;
    
    [Space]
    [SerializeField] private Color highlightColor = Color.green;
    
    [Space]
    [SerializeField] private CardArrangementModel arrangement;

    public SlotType Type => type;
    public uint Capacity => capacity;
    public SlotEntryway Entryway => entryway;
    public bool ShouldStartLocked => shouldStartLocked;
    public int Order => order;
    
    public Color HighlightColor => highlightColor;
    
    public CardArrangementModel Arrangement => arrangement;
}