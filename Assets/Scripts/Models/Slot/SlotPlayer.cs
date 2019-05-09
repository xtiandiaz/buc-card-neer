using UnityEngine;
using Zenject;

public class SlotPlayer : Slot
{
    public class Factory : PlaceholderFactory<IPile, Transform, Bounds, SlotPlayer>
    {
    }

    public SlotPlayer(IPile pile, Transform transform, Bounds bounds) 
        : base(SlotType.Player, pile, transform, bounds)
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