using DG.Tweening;

public struct LodgingSettings
{
    public static readonly LodgingSettings Default = new LodgingSettings(
        SlotLodgingMode.Default,
        false,
        Ease.OutQuart,
        0.5f);
    
    public static readonly LodgingSettings DefaultWithOthersArrangement = new LodgingSettings(
        SlotLodgingMode.Default,
        true,
        Ease.OutQuart,
        0.5f);
    
    public static readonly LodgingSettings Manual = new LodgingSettings(
        SlotLodgingMode.Manual,
        true,
        Ease.OutQuart,
        0.5f);

    public LodgingSettings(
        SlotLodgingMode mode,
        bool shouldRearrangeOthers,
        Ease ease,
        float duration
        )
    {
        Mode = mode;
        ShouldRearrangeOthers = shouldRearrangeOthers;
        Ease = ease;
        Duration = duration;
    }
    
    public SlotLodgingMode Mode { get; }
    public bool ShouldRearrangeOthers { get; }
    public Ease Ease { get; }
    public float Duration { get; }
}