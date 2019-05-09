using System;
using UniRx;
using UnityEngine;
using Zenject;

public interface ISea : ICardConsumer, IBoardSection
{
    ISlot[] Slots { get; }
    
    IObservable<bool> WhenToggledProjection { get; }
    
    void ToggleProjection(bool on);
}

public class Sea : ISea
{
    public class Factory : PlaceholderFactory<ISlot[], Sea>
    {
    }
    
    private readonly ReactiveProperty<bool> isProjected = new ReactiveProperty<bool>(true);
    private readonly Subject<ICard> consumption = new Subject<ICard>();
    private ICardProvider cardProvider;

    private Sea(ISlot[] slots)
    {
        Slots = slots;
    }

    public ISlot[] Slots { get; }
    
    public IObservable<bool> WhenToggledProjection => isProjected;
    public IObservable<ICard> WhenConsumed => consumption;

    public void Populate()
    {
        for (var i = 0; i < 3; i++)
        {
            foreach (var slot in Slots)
                Feed(slot);
        }
    }
    
    public void SetProvider(ICardProvider provider)
    {
        cardProvider = provider;
    }

    public void Feed(ISlot slot)
    {
        var card = cardProvider.Provide();
        
        slot.Lodge(card);
        
        consumption.OnNext(card);
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