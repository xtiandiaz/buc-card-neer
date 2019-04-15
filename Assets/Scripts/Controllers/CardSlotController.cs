using System;
using System.Linq;
using UniRx;
using Zenject;

public class CardSlotController
{
    public class Factory : PlaceholderFactory<ICardSlot, ICardSlotView, CardSlotController>
    {
    }
    
    private readonly ReactiveCollection<CardController> cards = new ReactiveCollection<CardController>(); 
    
    private CardSlotController(ICardSlot model, ICardSlotView view)
    {
        Model = model;
        View = view;
    }
    
    public ICardSlot Model { get; }
    public ICardSlotView View { get; }
    public bool DoesAcceptNewCards => cards.Count < Model.Capacity;

    public IObservable<Unit> Emptied => cards.ObserveCountChanged(true).Where(c => c == 0).AsUnitObservable();

    public void Take(CardController cardController)
    {
        if (!DoesAcceptNewCards)
            return;

        cards.Insert(0, cardController);
        
        ArrangeCards();
    }

    public CardController Release()
    {
        var firstItem = cards.FirstOrDefault();

        if (firstItem != null)
        {
            cards.RemoveAt(0);
            ArrangeCards();
        }

        return firstItem;
    }

    private void ArrangeCards()
    {
        var totalCards = cards.Count;
        for (var i = 0; i < totalCards; i++)
        {
            cards[i].Arrange(View.LocalPosition, i, totalCards - i);
        }
    }
}