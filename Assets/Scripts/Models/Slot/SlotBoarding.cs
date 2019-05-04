using Zenject;

public class SlotBoarding : Slot
{
    public class Factory : PlaceholderFactory<uint, SlotBoarding>
    {
    }

    public SlotBoarding(uint capacity) : base(SlotType.Boarding, capacity)
    {
    }

    public override CardType EntryMask => CardType.Pirate | CardType.Merchant | CardType.Resource;

    public override bool CanTake(ICard card, ISlot fromSlot)
    {
        return fromSlot.Type == SlotType.Event;
    }
}