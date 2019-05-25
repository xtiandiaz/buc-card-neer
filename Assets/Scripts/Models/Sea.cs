using Zenject;

public interface ISea : ICardProviderManager
{
    ISlot[] Slots { get; }

    void Clash();
    void Unlock();
}

public class Sea : ISea
{
    public class Factory : PlaceholderFactory<ISlot[], Sea>
    {
    }
    
    private readonly ICardProvider cardProvider;

    private Sea(
        ISlot[] slots,
        ICardProvider cardProvider
        )
    {
        Slots = slots;
        this.cardProvider = cardProvider;
    }

    public ISlot[] Slots { get; }

    public void AssignProviders()
    {
        foreach (var slot in Slots)
            slot.SetProvider(cardProvider);
    }

    public void Clash()
    {
        for (var i = 0; i < Slots.Length; i++)
        {
            var slotToClash = Slots[i];
            var previous = i - 1 >= 0 ? Slots[i - 1] : null;
            var next = i + 1 < Slots.Length ? Slots[i + 1] : null;
            
            if (slotToClash.IsLocked || slotToClash.Peek() == null)
                continue;

            if (previous?.IsLocked == false)
            {
                ICard previousCard;
                if ((previousCard = previous?.Peek()) != null)
                    slotToClash.Peek()?.Clash(previousCard);
            }

            if (next?.IsLocked == false)
            {
                ICard nextCard;
                if ((nextCard = next?.Peek()) != null)
                    slotToClash.Peek()?.Clash(nextCard);
            }
        }
    }

    public void Unlock()
    {
        foreach (var slot in Slots)
            slot.Unlock();
    }
}