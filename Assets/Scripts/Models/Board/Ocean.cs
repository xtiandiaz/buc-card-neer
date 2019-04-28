using System.Collections.Generic;
using Zenject;

public interface IOcean
{
    void Populate(IDeck fromDeck);
}

public class Ocean : IOcean
{
    private readonly ISlot[] slots;

    public class Factory : PlaceholderFactory<ISlot[], Ocean>
    {
    }

    private Ocean(ISlot[] slots)
    {
        this.slots = slots;
    }

    public void Populate(IDeck fromDeck)
    {
        foreach (var slot in slots)
        {
            var card = fromDeck.Supply();
            if (card == null)
                return;
            
            slot.Take(card);
        }
    }
}