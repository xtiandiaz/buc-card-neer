using Zenject;

public class SlotEvent : Slot
{
    public class Factory : PlaceholderFactory<uint, SlotEvent>
    {
    }

    public SlotEvent(uint capacity) : base(SlotType.Event, capacity)
    {
    }

    public override CardType EntryMask => CardType.Pirate | CardType.Merchant | CardType.Resource;

    public override bool CanTake(ICard card, ISlot fromSlot)
    {
        return fromSlot.Type == SlotType.Storage & card.Type == CardType.ArtilleryWeapon;
    }
}