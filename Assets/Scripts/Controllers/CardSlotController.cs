using System;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

public interface ICardSlotController
{
    uint Id { get; }
    CardSlotType Type { get; }
    Vector3 LocalPosition { get; }
    bool CanBeDealtOn { get; }
    ICardController Head { get; }

    bool DoesContain(Vector3 worldPoint);
    bool DoesContain(ICardController cardController);
    bool DoesAdmit(ICardController cardController);
    bool DoesMatch(ICardController cardController);
    bool Take(ICardController cardController);
    bool Release(ICardController cardController);
    void ToggleHighlight(bool on);
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
    public Vector3 LocalPosition => view.Transform.localPosition;
    public bool CanBeDealtOn => guests.Count < model.Capacity;
    public ICardController Head => guests.FirstOrDefault();
    
    public bool DoesContain(Vector3 worldPoint)
    {
        return view.DoesContain(worldPoint);
    }
    
    public bool DoesContain(ICardController cardController)
    {
        return guests.Contains(cardController);
    }
    
    public bool DoesAdmit(ICardController cardController)
    {
        return CanBeDealtOn 
               && cardController != null 
               && !DoesContain(cardController) 
               && model.Type == CardSlotType.Stash;
    }
    
    public bool DoesMatch(ICardController cardController)
    {
        return !DoesContain(cardController) && Head?.DoesMatch(cardController) == true;
    }

    public bool Take(ICardController dealtCardController)
    {
        if (!CanBeDealtOn)
            return false;
        
        guests.Insert(0, dealtCardController);
        
        dealtCardController.OnMoved(this);

        dealtCardController.Destroyed
            .Merge(dealtCardController.Moved)
            .Select(_ => dealtCardController)
            .Subscribe(c => Release(c))
            .AddTo(dealtCardController.Transform);
        
        ArrangeGuests();

        return true;
    }

    public bool Release(ICardController cardController)
    {
        var didReleaseCard = guests.Remove(cardController);
        
        if (didReleaseCard)
            ArrangeGuests();

        return didReleaseCard;
    }

    public void ToggleHighlight(bool on)
    {
        view.ToggleHighlight(on);
    }

    private void ArrangeGuests()
    {
        var totalGuests = guests.Count;
        for (var i = 0; i < totalGuests; i++)
        {
            guests[i].Arrange(i, totalGuests, view.Layout);
        }
    }
}