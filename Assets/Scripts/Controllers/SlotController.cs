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
    
    private static readonly Subject<(ICard, ISlot)> Picking = new Subject<(ICard, ISlot)>();
    private static readonly Subject<(ICard, ISlot, Vector3)> Dropping = new Subject<(ICard, ISlot, Vector3)>();
    private static readonly Subject<(ICard, ISlot)> Taking = new Subject<(ICard, ISlot)>();
    
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
        model.ArrangementSettings = view.ArrangementSettings;
        model.Position = view.Position;
        
        disposables.Add(model.Highlighting.Subscribe(view.ToggleHighlight));
        disposables.Add(model.Visibility.Subscribe(view.ToggleVisibility));
        disposables.Add(model.Taking.Subscribe(card => Taking.OnNext((card, model))));
        
        disposables.Add(Taking
            .Where(cardInSlot =>
            {
                var (card, slot) = cardInSlot;
                return slot != model && model.DoesContain(card);
            })
            .Select(cardInSlot => cardInSlot.Item1)
            .Subscribe(card => model.Release(card)));

        disposables.Add(Picking
            .Subscribe(cardFromSlot =>
            {
                var (card, slot) = cardFromSlot;
                model.ToggleHighlight(model.CanTake(card, slot));
            }));
        
        disposables.Add(Dropping
            .Do(_ => model.ToggleHighlight(false))
            .Where(cardFromSlotAtPosition => model.DoesContain(cardFromSlotAtPosition.Item3))
            .Subscribe(cardFromSlotAtPosition =>
            {
                var (card, slot, position) = cardFromSlotAtPosition;

                if (model.CanTake(card, slot))
                    model.Take(card);
            }));

        #region Dragging

        disposables.Add(view.DraggingStart
            .SkipWhile(_ => model.CardCount == 0)
            .Subscribe(_ =>
            {
                model.TopCard.Pick();
                    
                Picking.OnNext((model.TopCard, model));            
            }));
        
        disposables.Add(view.Dragging
            .SkipWhile(_ => model.CardCount == 0)
            .Subscribe(position => model.TopCard.Drag(position)));
        
        disposables.Add(view.DraggingEnd
            .SkipWhile(_ => model.CardCount == 0)
            .Subscribe(position =>
            {
                model.TopCard.Drop(position);
                model.ArrangeCards();
                    
                Dropping.OnNext((model.TopCard, model, position));
            }));

        #endregion
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}