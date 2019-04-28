using Zenject;

public class SlotDefense : Slot
{
    public class Factory : PlaceholderFactory<uint, SlotDefense>
    {
    }

    public SlotDefense(uint capacity) : base(SlotType.Defense, capacity)
    {
    }

    public override CardType EntryMask { get; }
}