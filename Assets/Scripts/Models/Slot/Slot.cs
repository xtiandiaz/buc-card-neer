using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

[Flags]
public enum SlotType
{
    Event     = 1 << 0,
    Boarding  = 1 << 1,
    Defense   = 1 << 2,
    Resource  = 1 << 3,
    Player    = 1 << 4,
    
    All = Event | Boarding | Defense | Resource | Player
}

public interface ISlot
{
    uint Capacity { get; }
    SlotType Type { get; }
    CardType EntryMask { get; }
    CardType MatchMask { get; }
    Vector3 Position { get; set; }
    IObservable<ICard> Took { get; }

    void Initialize(Vector3 atPosition);
    void Take(ICard card);
}

public abstract class Slot : ISlot
{
    private readonly List<ICard> cards = new List<ICard>();
    private readonly Subject<ICard> took = new Subject<ICard>();
    private Vector3 position;
    
    protected Slot(SlotType type, uint capacity)
    {
        Capacity = capacity;
        Type = type;
    }

    public abstract CardType EntryMask { get; }
    public uint Capacity { get; }
    public SlotType Type { get; }
    public CardType MatchMask => cards.Count > 0 ? cards.First().InteractionMask : EntryMask;

    public Vector3 Position
    {
        get => position;
        set
        {
            position = value;
            cards.ForEach(c => c.Position = position);
        }
    }

    public IObservable<ICard> Took => took;

    public void Initialize(Vector3 atPosition)
    {
        position = atPosition;
    }

    public void Take(ICard card)
    {
        cards.Add(card);
        took.OnNext(card);

        card.Position = Position;
    }
}