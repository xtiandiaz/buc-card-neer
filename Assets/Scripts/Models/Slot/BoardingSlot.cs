using UnityEngine;
using Zenject;

public interface IBoardingSlot : ISlot
{
}

public class BoardingSlot : Slot, IBoardingSlot
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
        return withCard.IsBoarded || (withCard.Type & CardType.Pirate) != 0;
    }

    protected override bool CanLodge(ISlot fromSlot)
    {
        return (fromSlot.Type & (SlotType.Supply | SlotType.Storage)) != 0;
    }

    protected override bool CanLodge(ICard card)
    {
        if (!card.IsBoarded)
            return (card.Type & (CardType.Resource | CardType.Agent)) != 0;
        
        return IsEmpty && (card.Type & CardType.WeaponRanged) != 0;
    }

    protected override void OnLodged(ICard card)
    {
        base.OnLodged(card);
        
        card.Board();
    }
}