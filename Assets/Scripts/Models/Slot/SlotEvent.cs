using Zenject;

public class SlotEvent : Slot
{
    public class Factory : PlaceholderFactory<uint, SlotEvent>
    {
    }

    public SlotEvent(uint capacity) : base(SlotType.Event, capacity)
    {
    }

    public override CardType EntryMask => CardType.Foe | CardType.Merchant | CardType.Resource;
}