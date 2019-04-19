using System;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

public interface ICardSlotController
{
    uint Id { get; }
    CardSlotType Type { get; }
    bool DoesAcceptNewGuests { get; }

    bool Lodge(ICardController cardController);
    bool Release(ICardController cardController);
    bool DoesContain(Vector3 worldPoint);
}

public class CardSlotController : ICardSlotController
{
    public class Factory : PlaceholderFactory<ICardSlot, ICardSlotView, CardSlotController>
    {
        private readonly CardSlot.Factory slotFactory;

        private Factory(
            CardSlot.Factory slotFactory 
            )
        {
            this.slotFactory = slotFactory;
        }

        public CardSlotController Create(ICardSlotView fromView)
        {
            var model = slotFactory.Create(fromView.Type, fromView.Capacity);
            
            return base.Create(model, fromView);
        }
    }
    
    private readonly ReactiveCollection<ICardController> guests = new ReactiveCollection<ICardController>();
    private readonly ICardSlot model;
    private readonly ICardSlotView view;

    private CardSlotController(
        ICardSlot model,
        ICardSlotView view
        )
    {
        this.model = model;
        this.view = view;
    }

    public uint Id => model.Id;
    public CardSlotType Type => model.Type; 
    
    public bool DoesAcceptNewGuests => guests.Count < model.Capacity;

    public bool Lodge(ICardController cardController)
    {
        if (!DoesAcceptNewGuests || cardController == null)
            return false;
        
        guests.Insert(0, cardController);
        
        ArrangeGuests();

        return true;
    }

    public bool DoesContain(Vector3 worldPoint)
    {
        return view.DoesContain(worldPoint);
    }

    public bool Release(ICardController cardController)
    {
        var didRemoveGuest = guests.Remove(cardController);
        
        if (didRemoveGuest)
            ArrangeGuests();

        return didRemoveGuest;
    }

    private void ArrangeGuests()
    {
        var totalGuests = guests.Count;
        for (var i = 0; i < totalGuests; i++)
        {
            guests[i].Arrange(view.Transform.localPosition, i);
        }
    }
}