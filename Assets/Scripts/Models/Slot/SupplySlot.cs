using UnityEngine;
using Zenject;

public class SupplySlot : Slot
{
    public class Factory : PlaceholderFactory<IPile, ISlotSettings, Bounds, Transform, SupplySlot>
    {
    }

    public SupplySlot(IPile pile, ISlotSettings settings, Bounds bounds, Transform transformBond) 
        : base(pile, settings, bounds, transformBond)
    {
    }

    protected override bool CanMatch(ICard withCard)
    {
        // Can't match against any Cards provided by the player
        return false;
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