using Zenject;

public class SlotResource : Slot
{
    public class Factory : PlaceholderFactory<uint, SlotResource>
    {
    }

    public SlotResource(uint capacity) : base(SlotType.Resource, capacity)
    {
    }

    public override CardType EntryMask { get; }
}