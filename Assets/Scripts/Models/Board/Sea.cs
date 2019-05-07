using System;
using UniRx;
using UnityEngine;
using Zenject;

public interface ISea
{
    ISlot[] Slots { get; }
    
    IObservable<bool> UpdatedProjectionState { get; }
    IObservable<IDeck> Dealing { get; }
    
    void Deal(IDeck fromDeck);
    void ToggleProjection(bool on);
}

public class Sea : ISea
{
    public class Factory : PlaceholderFactory<ISlot[], Sea>
    {
    }
    
    private readonly ReactiveProperty<bool> isProjected = new ReactiveProperty<bool>(true);
    private readonly Subject<IDeck> dealing = new Subject<IDeck>();

    private Sea(ISlot[] slots)
    {
        Slots = slots;
    }

    public ISlot[] Slots { get; }
    public IObservable<bool> UpdatedProjectionState => isProjected;
    public IObservable<IDeck> Dealing => dealing;

    public void Deal(IDeck fromDeck)
    {
        dealing.OnNext(fromDeck);
    }

    public void ToggleProjection(bool on)
    {
        isProjected.Value = on;

        foreach (var slot in Slots)
        {
            slot.IsVisible = on;
        }
    }
}