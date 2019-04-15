using System;
using System.Collections.Generic;
using UniRx;
using Zenject;

public interface ICardSlot
{
    int Capacity { get; }
    IObservable<bool> IsSelectedAsObservable { get; }
    IObservable<bool> IsLockedAsObservable { get; }
}

public class CardSlot : ICardSlot
{
    public class Factory : PlaceholderFactory<int, CardSlot>
    {
        public CardSlot Create(CardSlotView fromView)
        {
            return Create(fromView.InitialCapacity);
        }
    }
    
    private readonly ReactiveProperty<bool> isLocked = new ReactiveProperty<bool>();
    private readonly ReactiveProperty<bool> isSelected = new ReactiveProperty<bool>();

    private CardSlot(int capacity)
    {
        Capacity = capacity;
    }
    
    public int Capacity { get; }
    public Dictionary<Direction, CardSlot> Neighbors { get; } = new Dictionary<Direction, CardSlot>();
    public bool IsLocked => isLocked.Value;
    
    public IObservable<bool> IsSelectedAsObservable => isSelected;
    public IObservable<bool> IsLockedAsObservable => isLocked;
    

    public void Select()
    {
        isSelected.Value = true;
    }

    public void Deselect()
    {
        isSelected.Value = false;
    }

    public void Lock()
    {
        isLocked.Value = true;
    }
    
    public void Unlock()
    {
        isLocked.Value = false;
    }
}