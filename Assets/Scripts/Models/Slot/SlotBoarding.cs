using Zenject;

public class SlotBoarding : Slot
{
    public class Factory : PlaceholderFactory<IPile, SlotBoarding>
    {
    }

    public SlotBoarding(IPile pile) : base(SlotType.Boarding, pile)
    {
    }

    public override CardType EntryMask => CardType.Pirate | CardType.Merchant | CardType.Resource;

    public override bool CanLodge(ICard card, ISlot fromSlot)
    {
        return fromSlot.Type == SlotType.Event;
    }
}