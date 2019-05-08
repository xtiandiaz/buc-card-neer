using System;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

public interface ISlotController
{
}

public class SlotController : ISlotController, IDisposable
{
    public class Factory : PlaceholderFactory<ISlot, ISlotView, SlotController>
    {
    }
    
    private static readonly Subject<(ICard, ISlot)> CardPicking = new Subject<(ICard, ISlot)>();
    private static readonly Subject<(ICard, ISlot, Vector3)> CardDropping = new Subject<(ICard, ISlot, Vector3)>();
    
    private readonly ISlot model;
    private readonly ISlotView view;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private SlotController(ISlot model, ISlotView view)
    {
        this.model = model;
        this.view = view;
    }

    [Inject]
    private void Initialize()
    {
        model.Position = view.Position;
        model.Entryway = view.Entryway;
        model.Bounds = view.Bounds;
        model.IsLocked = view.ShouldStartLocked;
        
        disposables.Add(model.Highlighting.Subscribe(view.ToggleHighlight));
        disposables.Add(model.Visibility.Subscribe(view.ToggleVisibility));
        
        #region Picking

        disposables.Add(model.WhenCardPicked
            // Push Card picking from Slot:
            .Subscribe(card => CardPicking.OnNext((card, model))));
        
        disposables.Add(CardPicking
            .Subscribe(cardFromSlot =>
            {
                var (card, slot) = cardFromSlot;
                model.ToggleHighlight(model.CanLodge(card, slot));
            }));

        #endregion
        
        #region Dragging

        disposables.Add(view.WhenDraggingStarted
            .SkipWhile(_ => model.IsLocked)
            .Take(1)
            .Select(_ => model.Pick())
            .ContinueWith(pickedCard => view.WhenDragged
                .TakeUntil(view.WhenDraggingStopped)
                .Do(pickedCard.Drag)
                .Last()
                .Select(lastDraggingPosition => new { Card = pickedCard, Position = lastDraggingPosition }))
            .RepeatSafe()
            .Subscribe(droppedCardAtPosition =>
            {
                droppedCardAtPosition.Card.Drop();
                model.Arrange();
                
                // Push Card dropping from Slot at Position:
                CardDropping.OnNext((droppedCardAtPosition.Card, model, droppedCardAtPosition.Position));
            }));

        #endregion
        
        #region Dropping
        
        disposables.Add(CardDropping
            .Do(_ => model.ToggleHighlight(false))
            .Where(cardFromSlotAtPosition => model.DoesContain(cardFromSlotAtPosition.Item3))
            .Subscribe(cardFromSlotAtPosition =>
            {
                var (card, slot, position) = cardFromSlotAtPosition;

                if (model.CanLodge(card, slot))
                    model.Lodge(card);
            }));

        #endregion
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}