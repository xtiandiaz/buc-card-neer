using System;
using UniRx;
using UnityEngine;
using Zenject;

public interface ISea
{
    ISlot[] Slots { get; }
    
    IObservable<bool> UpdatedProjectionState { get; }
    
    void Populate(IDeck fromDeck);
    void ToggleProjection(bool on);
}

public class Sea : ISea
{
    public class Factory : PlaceholderFactory<ISlot[], Sea>
    {
    }
    
    private readonly ReactiveProperty<bool> isProjected = new ReactiveProperty<bool>(true);

    private Sea(ISlot[] slots)
    {
        Slots = slots;
    }

    public ISlot[] Slots { get; }
    public IObservable<bool> UpdatedProjectionState => isProjected;

    public void Populate(IDeck fromDeck)
    {
        foreach (var slot in Slots)
        {
            for (var i = 0; i < slot.Capacity; i++)
            {
                if (!slot.HasRoom)
                    break;
                
                var card = fromDeck.Supply(true);
                if (card == null)
                {
                    Debug.LogWarning($"[Sea] {fromDeck.Type} Deck supplied null Card.");
                    return;
                }

                slot.Lodge(card);
            }
        }
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