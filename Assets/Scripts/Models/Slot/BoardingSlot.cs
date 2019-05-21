using UnityEngine;
using Zenject;

public class BoardingSlot : Slot
{
    public class Factory : PlaceholderFactory<IPile, ISlotSettings, Bounds, Transform, BoardingSlot>
    {
    }

    public BoardingSlot(IPile pile, ISlotSettings settings, Bounds bounds, Transform transformBond) 
        : base(pile, settings, bounds, transformBond)
    {
    }
    
    protected override bool CanMatch(ICard withCard)
    {
        return withCard.IsBoarded;
    }

    protected override bool CanLodge(ISlot fromSlot)
    {
        return fromSlot.Type == SlotType.Supply;
    }

    protected override bool CanLodge(ICard card)
    {
        return (card.Type & (CardType.Resource | CardType.Agent)) != 0;
    }

    protected override void OnLodged(ICard card)
    {
        base.OnLodged(card);
        
        card.Board();
    }
}