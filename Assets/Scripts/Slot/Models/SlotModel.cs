using UnityEngine;

public interface ISlotModel
{
    SlotType Type { get; }
    uint Capacity { get; }
    int Index { get; }
    bool ShouldStartLocked { get; }
    bool ShouldSelfArrange { get; }

    Color HighlightColor { get; }
    
    CardArrangementModel Arrangement { get; }
}

[CreateAssetMenu(menuName = "Model/Slot")]
public class SlotModel : ScriptableObject, ISlotModel
{
    [SerializeField] private SlotType type = default;
    [SerializeField] private int index = default;
    [SerializeField] private uint capacity = default;
    [SerializeField] private bool shouldStartLocked = default;
    [SerializeField] private bool shouldSelfArrange = default;

    [Space]
    [SerializeField] private Color highlightColor = Color.green;
    
    [Space]
    [SerializeField] private CardArrangementModel arrangement = default;

    public SlotType Type => type;
    public uint Capacity => capacity;
    public int Index => index;
    public bool ShouldStartLocked => shouldStartLocked;
    public bool ShouldSelfArrange => shouldSelfArrange;

    public Color HighlightColor => highlightColor;
    
    public CardArrangementModel Arrangement => arrangement;
}