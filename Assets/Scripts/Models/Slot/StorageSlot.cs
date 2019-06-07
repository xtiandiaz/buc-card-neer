using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public interface IStorageSlot : ISlot
{
    ResourceType ResourceMask { get; }

    void Sort();
}

public class StorageSlot : Slot, IStorageSlot
{
    public class Factory : PlaceholderFactory<IPile, ISlotSettings, Bounds, Transform, StorageSlot>
    {
    }

    private Queue<CardType> sortingQueue;

    public StorageSlot(IPile pile, ISlotSettings settings, Bounds bounds, Transform transformBond) 
        : base(pile, settings, bounds, transformBond)
    {
    }

    public ResourceType ResourceMask => Settings.ResourceMask;

    public void Sort()
    {
        if (pile.Count < 1)
            return;

        var card = pile.Peek();

        pile.Remove(card);
        
        pile.Insert(card, PileInsertionMode == PileInsertionMode.Push ? PileInsertionMode.Unshift : PileInsertionMode.Push);
        
        Arrange();
    }

    public override void Lodge(ICard card)
    {
        base.Lodge(card);
        
        sortingQueue = new Queue<CardType>(pile.Types);
    }

    public override void Release(ICard card)
    {
        base.Release(card);
        
        sortingQueue = new Queue<CardType>(pile.Types);
    }

    public override bool CanDefer(ICard card)
    {
        return false;
    }

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