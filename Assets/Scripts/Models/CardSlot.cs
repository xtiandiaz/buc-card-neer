using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Zenject;

public enum CardSlotType
{
    Event,
    Encounter,
    Defense,
    Resource
}

public interface ICardSlot
{
    uint Capacity { get; }
    CardSlotType Type { get; }
}

public class CardSlot : ICardSlot
{
    public class Factory : PlaceholderFactory<CardSlotType, uint, CardSlot>
    {
    }

    private CardSlot(CardSlotType type, uint capacity)
    {
        Capacity = capacity;
        Type = type;
    }

    public uint Capacity { get; }
    public CardSlotType Type { get; }
}