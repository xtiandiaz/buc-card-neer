using Zenject;

public class SlotPlayer : Slot
{
    public class Factory : PlaceholderFactory<IPile, SlotPlayer>
    {
    }

    public SlotPlayer(IPile pile) : base(SlotType.Player, pile)
    {
    }

    protected override bool CanLodge(ISlot fromSlot)
    {
        return false;
    }

    protected override bool CanLodge(ICard card)
    {
        return (card.Type & CardType.Player) != 0;
    }
}