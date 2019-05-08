using Zenject;

public class SlotEvent : Slot
{
    public class Factory : PlaceholderFactory<IPile, SlotEvent>
    {
    }

    public SlotEvent(IPile pile) : base(SlotType.Event, pile)
    {
    }

    protected override bool CanLodge(ISlot fromSlot)
    {
        // Can't lodge from any other Slot but only when dealt on directly
        return false;
    }

    protected override bool CanLodge(ICard card)
    {
        return (card.Type & (CardType.Resource | CardType.Pirate | CardType.Merchant)) != 0;
    }
}