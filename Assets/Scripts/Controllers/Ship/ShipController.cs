using System;
using UniRx;
using UnityEngine;
using Zenject;

public interface IShipController
{
    void Initialize();
}

public class ShipController : IShipController, IDisposable
{
    public class Factory : PlaceholderFactory<IShip, IShipView, ShipController>
    {
    }
    
    private readonly IShip model;
    private readonly IShipView view;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private ShipController(IShip model, IShipView view)
    {
        this.model = model;
        this.view = view;
    }

    public void Initialize()
    {
        var viewHidingPosition = Vector3.up * (view.ViewportHeight + view.Height * 0.5f);
        
        disposables.Add(model.Lodged.Subscribe(OnLodged));
        disposables.Add(model.Docked.Subscribe(atPosition => view.Dock(atPosition)));
        disposables.Add(model.Sailed.Subscribe(_ => view.SetSail(viewHidingPosition)));
    }

    public void Dispose()
    {
        disposables?.Dispose();
    }
    
    private void OnLodged((ICard, ISlot) cardInSlot)
    {
        var (card, slot) = cardInSlot;

        switch (slot.Type)
        {
            case SlotType.Boarding:
                
                card.Flip(CardFace.Front);
                
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}