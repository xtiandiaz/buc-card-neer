using System;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

public interface ISlotController
{
    void Initialize();
}

public class SlotController : ISlotController, IDisposable
{
    public class Factory : PlaceholderFactory<ISlot, ISlotView, SlotController>
    {
    }
    
    private static readonly Subject<(ICard, ISlot)> Lodged = new Subject<(ICard, ISlot)>();
    private readonly ISlot model;
    private readonly ISlotView view;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private SlotController(ISlot model, ISlotView view)
    {
        this.model = model;
        this.view = view;
    }

    public void Initialize()
    {
        model.Bounds = view.Bounds;
        model.Arrangement = view.Arrangement;
        model.Position = view.Position;
        
        disposables.Add(model.Lodged.Subscribe(card => Lodged.OnNext((card, model))));
        
        disposables.Add(Lodged
            .Where(cardInSlot => cardInSlot.Item2 != model && model.DoesContain(cardInSlot.Item1))
            .Subscribe(cardInSlot => model.Release(cardInSlot.Item1)));
        
        disposables.Add(model.BecameHighlighted.Subscribe(view.ToggleHighlight));
        disposables.Add(model.BecameVisible.Subscribe(view.ToggleVisibility));
    }

    public void Dispose()
    {
        disposables?.Dispose();
    }
}