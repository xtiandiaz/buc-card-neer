using UnityEngine;
using Zenject;

public interface ISlotStorage : ISlot
{
    ResourceType ResourceMask { get; }
}

public class SlotStorage : Slot, ISlotStorage
{
    public class Factory : PlaceholderFactory<IPile, ISlotSettings, Bounds, Transform, SlotStorage>
    {
    }

    public SlotStorage(IPile pile, ISlotSettings settings, Bounds bounds, Transform transformBond) 
        : base(pile, settings, bounds, transformBond)
    {
    }

    public ResourceType ResourceMask => settings.ResourceMask;

    protected override bool CanLodge(ISlot fromSlot)
    {
        return (fromSlot.Type & (SlotType.Boarding)) != 0;
    }

    protected override bool CanLodge(ICard card)
    {
        if (!(card is ICardResource resourceCard))
            return false;

        if ((ResourceMask & resourceCard.ResourceType) == 0)
            return false;

        return true;
    }
}