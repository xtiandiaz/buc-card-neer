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

    public override CardType EntryMask => CardType.Resource;
    public ResourceType ResourceMask { get; }

    public override bool CanLodge(ICard card)
    {
        if (card is IResourceCard resourceCard)
            return CanTake(resourceCard);

        return false;
    }
    
    public override bool CanLodge(ICard card, ISlot fromSlot)
    {
        switch (fromSlot.Type)
        {
            case SlotType.Boarding:

                if (card is IResourceCard resourceCard)
                    return resourceCard.IsTreasure || resourceCard.WasPaidFor;
                
                break;
        }

        return false;
    }

    private bool CanTake(IResourceCard resourceCard)
    {
        return (ResourceMask & resourceCard.ResourceType) != 0;
    }
}