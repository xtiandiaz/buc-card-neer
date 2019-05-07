using Zenject;

public class SlotPlayer : Slot
{
    public class Factory : PlaceholderFactory<IPile, SlotPlayer>
    {
    }

    public SlotPlayer(IPile pile) : base(SlotType.Player, pile)
    {
    }

    public override CardType EntryMask => CardType.Player | CardType.Pirate;

    public override bool CanLodge(ICard card, ISlot fromSlot)
    {
        return (fromSlot.Type & SlotType.Boarding) != 0;
    }
}