using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using UniRx;

public interface IBoardController
{
    void Initialize();
    void OnPicked(ICard card);
    void OnDropped(ICard card);
}

public class BoardController : IBoardController, IDisposable
{
    public class Factory : PlaceholderFactory<IBoard, IBoardView, BoardController>
    {
    }
    
    private readonly IBoard model;
    private readonly IBoardView view;
    private readonly GameSettings settings;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private BoardController(
        IBoard model,
        IBoardView view
        )
    {
        this.model = model;
        this.view = view;
    }

    public void Initialize()
    {
        disposables.Add(
            model.CardPicked.Subscribe(OnPicked));
        
        disposables.Add(
            model.CardDropped.Subscribe(OnDropped));
    }

    public void OnPicked(ICard card)
    {
        foreach (var slot in model.PlaySlots)
        {
            slot.ToggleHighlight(slot.InteractionMask.Contains(card.Type));
        }
    }
    
    public void OnDropped(ICard card)
    {
        foreach (var slot in model.PlaySlots)
        {
            slot.ToggleHighlight(false);
        }
    }

    public void Dispose()
    {
        disposables?.Dispose();
    }
}