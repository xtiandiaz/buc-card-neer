using System;
using UniRx;
using UnityEngine;
using Zenject;

public interface ISlotController : IInitializable, IDisposable
{
}

public class SlotController : ISlotController
{
    public class Factory : PlaceholderFactory<ISlot, ISlotView, SlotController>
    {
    }
    
    protected readonly ISlot model;
    protected readonly ISlotView view;
    protected readonly CompositeDisposable disposables = new CompositeDisposable();
    
    private static readonly Subject<(ICard, ISlot)> CardPicking = new Subject<(ICard, ISlot)>();
    private static readonly Subject<(ICard, ISlot, Vector3)> CardDropping = new Subject<(ICard, ISlot, Vector3)>();
    
    [Inject] private IMoveRouter moveRouter;

    protected SlotController(ISlot model, ISlotView view)
    {
        this.model = model;
        this.view = view;
    }

    [Inject]
    public virtual void Initialize()
    {        
        disposables.Add(model.WhenToggledHighlighting.Subscribe(view.ToggleHighlight));
        
        #region Picking

        disposables.Add(model.WhenPicked
            // Push Card picking from Slot:
            .Subscribe(card => CardPicking.OnNext((card, model))));
        
        disposables.Add(CardPicking
            .Where(cardFromSlot => model != cardFromSlot.Item2)
            .Subscribe(cardFromSlot =>
            {
                var (card, slot) = cardFromSlot;
                model.ToggleHighlight(model.CanMatch(card, slot) || model.CanLodge(card, slot));
            }));

        #endregion
        
        #region Dragging

        disposables.Add(view.WhenDraggingStarted
            .SkipWhile(_ => model.IsLocked || model.IsEmpty)
            .Take(1)
            .Select(_ => model.Pick())
            .ContinueWith(pickedCard => view.WhenDragged
                .TakeUntil(view.WhenDraggingStopped)
                .Do(pickedCard.Drag)
                .Last()
                .Select(_ => new
                {
                    Card = pickedCard,
                    Position = new Vector3(
                        model.Position.x + pickedCard.LocalPosition.x,
                        model.Position.y + pickedCard.LocalPosition.y)
                }))
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
            .Where(cardFromSlotAtPosition => 
                model != cardFromSlotAtPosition.Item2 && model.DoesContain(cardFromSlotAtPosition.Item3))
            .Subscribe(cardFromSlotAtPosition =>
            {
                var (card, slot, position) = cardFromSlotAtPosition;

                if (model.CanMatch(card, slot))
                {
                    model.Match(card);
                    moveRouter.OnNext();
                }
                else if (model.CanLodge(card, slot))
                {
                    model.Lodge(card);
                    moveRouter.OnNext();
                }
            }));

        #endregion

        #region Arrangement

        disposables.Add(model.WhenLodged.Subscribe(_ => model.Arrange()));

        #endregion
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}