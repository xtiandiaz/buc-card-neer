using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using UniRx;
using Debug = System.Diagnostics.Debug;

public interface IBoardController
{
    void Initialize();
    void OnPicked(ICard card);
    void OnDropped((ICard, Vector3) cardAtPosition);
    void OnBoardedPlayer(ICard withCard);
}

public class BoardController : IBoardController, IDisposable
{
    public class Factory : PlaceholderFactory<IBoard, IBoardView, BoardController>
    {
    }
    
    private readonly Board model;
    private readonly BoardView view;
    private readonly GameSettings settings;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private BoardController(
        Board model,
        BoardView view
        )
    {
        this.model = model;
        this.view = view;
    }

    public void Initialize()
    {
        disposables.Add(model.CardPicked.Subscribe(OnPicked));
        disposables.Add(model.CardDropped.Subscribe(OnDropped));
        disposables.Add(model.ShipPlayer.Boarded.Subscribe(OnBoardedPlayer));
    }

    public void OnPicked(ICard card)
    {
        foreach (var slot in model.PlaySlots)
        {
            slot.ToggleHighlight(slot.CanLodge(card));
        }
    }
    
    public void OnDropped((ICard, Vector3) cardAtPosition)
    {
        foreach (var slot in model.PlaySlots)
        {
            slot.ToggleHighlight(false);
        }

        var (card, dropPosition) = cardAtPosition;

        model.PlaySlots.FirstOrDefault(s => s.DoesContain(dropPosition) && s.CanLodge(card))?
            .Lodge(card);
    }

    public void OnBoardedPlayer(ICard withCard)
    {
        switch (withCard.Type)
        {
            case CardType.Pirate:
                
                model.Sea.ToggleProjection(false);
                model.ShipPirate.Dock(view.PirateDockingPosition);
                
                break;
            case CardType.Merchant:
                
                model.Sea.ToggleProjection(false);
                model.ShipMerchant.Dock(view.MerchantDockingPosition);
                
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Dispose()
    {
        disposables?.Dispose();
    }
}