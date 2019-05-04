using Zenject;

public interface ISlotStorage : ISlot
{
    ResourceType ResourceMask { get; }
}

public class SlotStorage : Slot, ISlotStorage
{
    public class Factory : PlaceholderFactory<ResourceType, uint, SlotStorage>
    {
    }

    public SlotStorage(ResourceType resourceMask, uint capacity) : base(SlotType.Storage, capacity)
    {
        ResourceMask = resourceMask;
    }

    public override CardType EntryMask => CardType.Resource;
    public ResourceType ResourceMask { get; }

    public override bool CanTake(ICard card)
    {
        if (card is IResourceCard resourceCard)
            return CanTake(resourceCard);

        return false;
    }
    
    public override bool CanTake(ICard card, ISlot fromSlot)
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