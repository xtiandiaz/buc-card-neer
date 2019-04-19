using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Zenject;

public enum CardSlotType
{
    Play,
    Player,
    Stash
}

public interface ICardSlot
{
    uint Id { get; }
    uint Capacity { get; }
    CardSlotType Type { get; }

    bool Take(ICard card);
    ICard Release();
}

public class CardSlot : ICardSlot
{
    public class Factory : PlaceholderFactory<uint, CardSlotType, uint, CardSlot>
    {
        private static uint serialNumber = 0;
        
        public CardSlot Create(CardSlotType type, uint capacity)
        {
            return base.Create(serialNumber++, type, capacity);
        }
    }
    
    private readonly ReactiveCollection<ICard> cards = new ReactiveCollection<ICard>();

    private CardSlot(uint id, CardSlotType type, uint capacity)
    {
        Id = id;
        Capacity = capacity;
        Type = type;
    }

    public uint Id { get; }
    public uint Capacity { get; }
    public CardSlotType Type { get; }

    public bool Take(ICard card)
    {
        if (cards.Count >= Capacity)
            return false;
            
        cards.Insert(0, card);

        return true;
    }

    public ICard Release()
    {
        var card = cards.FirstOrDefault();

        if (card != null)
            cards.RemoveAt(0);

        return card;
    }
}