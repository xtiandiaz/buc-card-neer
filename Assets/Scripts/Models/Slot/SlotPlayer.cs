using Zenject;

public class SlotPlayer : Slot
{
    public class Factory : PlaceholderFactory<uint, SlotPlayer>
    {
    }

    public SlotPlayer(uint capacity) : base(SlotType.Player, capacity)
    {
    }

    public override CardType EntryMask => CardType.Player | CardType.Pirate;

    public override bool CanTake(ICard card, ISlot fromSlot)
    {
        return (fromSlot.Type & SlotType.Boarding) != 0;
    }
}