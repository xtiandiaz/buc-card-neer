using Zenject;

public class SlotEvent : Slot
{
    public class Factory : PlaceholderFactory<IPile, SlotEvent>
    {
    }

    public SlotEvent(IPile pile) : base(SlotType.Event, pile)
    {
    }

    public override CardType EntryMask => CardType.Pirate | CardType.Merchant | CardType.Resource;

    public override bool CanLodge(ICard card, ISlot fromSlot)
    {
        return fromSlot.Type == SlotType.Storage & card.Type == CardType.WeaponArtillery;
    }
}