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

    protected override bool CanLodge(ISlot fromSlot)
    {
        return false;
    }

    protected override bool CanLodge(ICard card)
    {
        return (card.Type & CardType.Player) != 0;
    }
}