using System;
using UniRx;
using UnityEngine;
using Zenject;

public interface ISea : ICardProviderManager
{
    ISlot[] Slots { get; }
    
    IObservable<bool> WhenToggledProjection { get; }
    
    void ToggleProjection(bool toValue);
}

public class Sea : ISea
{
    public class Factory : PlaceholderFactory<ISlot[], Sea>
    {
    }
    
    private readonly ICardProvider cardProvider;
    private readonly Subject<bool> projection = new Subject<bool>();

    private Sea(
        ISlot[] slots,
        ICardProvider cardProvider
        )
    {
        Slots = slots;
        this.cardProvider = cardProvider;
    }

    public ISlot[] Slots { get; }
    
    public IObservable<bool> WhenToggledProjection => projection;

    public void AssignProviders()
    {
        foreach (var slot in Slots)
            slot.SetProvider(cardProvider);
    }

    public void ToggleProjection(bool toValue)
    {
        foreach (var slot in Slots)
        {
            slot.IsVisible = toValue;
        }
        
        projection.OnNext(toValue);
    }
}