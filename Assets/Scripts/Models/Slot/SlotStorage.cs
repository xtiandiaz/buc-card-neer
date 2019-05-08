using Zenject;

public interface ISlotStorage : ISlot
{
    ResourceType ResourceMask { get; }
}

public class SlotStorage : Slot, ISlotStorage
{
    public class Factory : PlaceholderFactory<ResourceType, IPile, SlotStorage>
    {
    }

    public SlotStorage(ResourceType resourceMask, IPile pile) : base(SlotType.Storage, pile)
    {
        ResourceMask = resourceMask;
    }

    public ResourceType ResourceMask { get; }

    protected override bool CanLodge(ISlot fromSlot)
    {
        return (fromSlot.Type & (SlotType.Boarding)) != 0;
    }

    protected override bool CanLodge(ICard card)
    {
        if (!(card is IResourceCard resourceCard))
            return false;

        if ((ResourceMask & resourceCard.ResourceType) == 0)
            return false;

        return true;
        //return resourceCard.IsTreasure || resourceCard.WasPaidFor;
    }
}