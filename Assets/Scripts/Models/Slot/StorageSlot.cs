using UnityEngine;
using Zenject;

public interface IStorageSlot : ISlot
{
    ResourceType ResourceMask { get; }
}

public class StorageSlot : Slot, IStorageSlot
{
    public class Factory : PlaceholderFactory<IPile, ISlotSettings, Bounds, Transform, StorageSlot>
    {
    }

    public StorageSlot(IPile pile, ISlotSettings settings, Bounds bounds, Transform transformBond) 
        : base(pile, settings, bounds, transformBond)
    {
    }

    public ResourceType ResourceMask => settings.ResourceMask;

    protected override bool CanMatch(ICard withCard)
    {
        return false;
    }
    
    protected override bool CanLodge(ISlot fromSlot)
    {
        // Can store only what's on-board
        return (fromSlot.Type & SlotType.Boarding) != 0;
    }

    protected override bool CanLodge(ICard card)
    {
        if (!card.IsBoarded)
            return false;
        
        if (!(card is IResourceCard resourceCard) || resourceCard.IsWrapped || (ResourceMask & resourceCard.ResourceType) == 0)
            return false;

        return true;
    }
}