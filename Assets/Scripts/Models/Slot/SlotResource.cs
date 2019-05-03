using Zenject;

public interface ISlotResource : ISlot
{
    ResourceType ResourceMask { get; }
}

public class SlotResource : Slot, ISlotResource
{
    public class Factory : PlaceholderFactory<ResourceType, uint, SlotResource>
    {
    }

    public SlotResource(ResourceType resourceMask, uint capacity) : base(SlotType.Resource, capacity)
    {
        ResourceMask = resourceMask;
    }

    public override CardType EntryMask { get; }
    public ResourceType ResourceMask { get; }

    public override bool CanLodge(ICard card)
    {
        if (card is IResourceCard resCard)
            return (ResourceMask & resCard.ResourceType) != 0;

        return base.CanLodge(card);
    }
}