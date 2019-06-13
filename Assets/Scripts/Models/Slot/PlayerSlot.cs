using UnityEngine;
using Zenject;

public class PlayerSlot : Slot
{
    public class Factory : PlaceholderFactory<IPile, ISlotSettings, Bounds, Transform, PlayerSlot>
    {
    }

    public PlayerSlot(IPile pile, ISlotSettings settings, Bounds bounds, Transform transformBond) 
        : base(pile, settings, bounds, transformBond)
    {
    }

    public override bool CanDefer(ICard card)
    {
        return false;
    }

    protected override bool CanMatch(ICard withCard)
    {
        return withCard.IsBoarded;
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