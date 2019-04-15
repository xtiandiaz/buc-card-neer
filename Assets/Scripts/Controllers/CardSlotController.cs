using System.Collections.Generic;
using Zenject;

public class CardSlotController
{
    public class Factory : PlaceholderFactory<ICardSlot, ICardSlotView, CardSlotController>
    {
    }
    
    private readonly Stack<CardController> cards;
    
    private CardSlotController(ICardSlot model, ICardSlotView view)
    {
        Model = model;
        View = view;

        cards = new Stack<CardController>(model.Capacity);
    }
    
    public ICardSlot Model { get; }
    public ICardSlotView View { get; }
    public bool DoesAcceptNewCards => cards.Count < Model.Capacity;

    public void Take(CardController cardController)
    {
        if (!DoesAcceptNewCards)
            return;
        
        cards.Push(cardController);

        cardController.Locate(View.LocalPosition);
    }

    public CardController Release()
    {
        return cards.Pop();
    }
}