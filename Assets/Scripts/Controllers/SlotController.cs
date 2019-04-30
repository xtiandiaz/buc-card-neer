using System;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

public interface ISlotController
{
    void Initialize();
    void Arrange();
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
        model.Arrangement = view.Arrangement;

        disposables.Add(
            model.Lodged.Do(card =>
                {
                    card.Lodge(view.Transform);
                    Arrange();
                })
                .SelectMany(card => card.Lodged.Skip(1).Take(1).Select(_ => card))
                .Subscribe(card =>
                {
                    model.Release(card);
                    Arrange();
                }));
        
        disposables.Add(model.BecameHighlighted.Subscribe(view.ToggleHighlight));
        disposables.Add(model.BecameVisible.Subscribe(view.ToggleVisibility));
    }

    public void Arrange()
    {
        var cards = model.Cards;
        var cardCount = cards.Length;
        
        for (var i = 0; i < cardCount; i++)
            model.Arrangement.Arrange(cards[i], i, cardCount);
    }

    public void Dispose()
    {
        disposables?.Dispose();
    }
}