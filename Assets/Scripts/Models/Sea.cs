using System;
using UniRx;
using UnityEngine;
using Zenject;

public interface ISea : ICardProviderManager
{
    ISlot[] Slots { get; }
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
}