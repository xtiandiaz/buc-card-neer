using Zenject;

public class SlotBoarding : Slot
{
    public class Factory : PlaceholderFactory<IPile, SlotBoarding>
    {
    }

    public SlotBoarding(IPile pile) : base(SlotType.Boarding, pile)
    {
    }

    protected override bool CanLodge(ISlot fromSlot)
    {
        return fromSlot.Type == SlotType.Event;
    }

    protected override bool CanLodge(ICard card)
    {
        return (card.Type & (CardType.Resource | CardType.Pirate | CardType.Merchant)) != 0;
    }
}