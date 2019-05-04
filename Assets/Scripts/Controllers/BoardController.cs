using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using UniRx;
using UnityEngine.Tilemaps;
using Debug = System.Diagnostics.Debug;

public interface IBoardController
{
    void Initialize();
}

public class BoardController : IBoardController, IDisposable
{
    public class Factory : PlaceholderFactory<IBoard, IBoardView, BoardController>
    {
    }
    
    private readonly Board model;
    private readonly BoardView view;
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
        disposables.Add(model.ShipPlayer.Boarded
            .Where(card => (card.Type & (CardType.Pirate | CardType.Merchant)) != 0)
            .Subscribe(card =>
            {
                switch (card.Type)
                {
                    case CardType.Pirate:
                
                        model.ShipPirate.Dock(view.PirateDockingPosition);
                
                        break;
                    case CardType.Merchant:
                
                        model.ShipMerchant.Dock(view.MerchantDockingPosition);
                
                        break;
                }
                
                model.Sea.ToggleProjection(false);
            }));
    }

    public void Dispose()
    {
        disposables?.Dispose();
    }
}