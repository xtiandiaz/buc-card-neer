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
        model.Arrangement = view.CardArrangement;
        model.Position = view.Position;
        model.SlottingFace = view.SlottingFace;
        model.IsLocked = view.ShouldStartLocked;
        
        disposables.Add(model.Highlighting.Subscribe(view.ToggleHighlight));
        disposables.Add(model.Visibility.Subscribe(view.ToggleVisibility));

        #region Picking & Dropping

        disposables.Add(model.WhenCardPicked
            .Subscribe(cardFromSlot =>
            {
                var (card, slot) = cardFromSlot;
                model.ToggleHighlight(model.CanLodge(card, slot));
            }));
        
        disposables.Add(model.WhenCardDropped
            .Do(_ => model.ToggleHighlight(false))
            .Where(cardFromSlotAtPosition => model.DoesContain(cardFromSlotAtPosition.Item3))
            .Subscribe(cardFromSlotAtPosition =>
            {
                var (card, slot, position) = cardFromSlotAtPosition;

                if (model.CanLodge(card, slot))
                    model.Lodge(card);
            }));

        #endregion

        #region Dragging

        disposables.Add(view.WhenStartedDragging
            .SkipWhile(_ => model.IsLocked)
            .Select(_ => model.Pick())
            .SelectMany(pickedCard => view.WhenDragged.Do(pickedCard.Drag))
            .Subscribe());

        #endregion
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}