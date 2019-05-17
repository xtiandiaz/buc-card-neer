using UnityEngine;
using Zenject;

public class EventSlot : Slot
{
    public class Factory : PlaceholderFactory<IPile, ISlotSettings, Bounds, Transform, EventSlot>
    {
    }

    public EventSlot(IPile pile, ISlotSettings settings, Bounds bounds, Transform transformBond) 
        : base(pile, settings, bounds, transformBond)
    {
    }

    protected override bool CanLodge(ISlot fromSlot)
    {
        // Can't lodge from any other Slot but only when dealt on directly
        return false;
    }

    protected override bool CanLodge(ICard card)
    {
        return (card.Type & (CardType.Resource | CardType.Pirate | CardType.Merchant | CardType.Inspector)) != 0;
    }
}