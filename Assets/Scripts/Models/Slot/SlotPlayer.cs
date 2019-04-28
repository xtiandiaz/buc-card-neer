using Zenject;

public class SlotPlayer : Slot
{
    public class Factory : PlaceholderFactory<uint, SlotPlayer>
    {
    }

    public SlotPlayer(uint capacity) : base(SlotType.Player, capacity)
    {
    }

    public override CardType EntryMask { get; }
}