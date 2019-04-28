using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using UniRx;

public interface IBoardController
{
}

public class BoardController : IBoardController, IInitializable, IDisposable
{
    public class Factory : PlaceholderFactory<IBoard, IBoardView, BoardController>
    {
    }
    
    private readonly IBoard model;
    private readonly IBoardView view;
    private readonly GameSettings settings;
    private readonly CompositeDisposable perGameDisposables = new CompositeDisposable();

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
        //shipController.Board(cardControllerFactory.Create(model.Deck.Supply()));
    }

    public void Dispose()
    {
        perGameDisposables.Dispose();
    }

    /*public void Deal(ICardSlotController onSlot, int count)
    {
        for (var i = 0; i < count; i++)
        {
            if (!onSlot.CanBeDealtOn)
                break;
            
            var card = deckController.Draw();
            if (card == null)
                break;

            Place(card, onSlot);
        }
    }

    public void Place(ICardController card, ICardSlotController onSlot)
    {
        onSlot.Take(card);

        if (!card.IsDraggable)
            return;
            
        card.InteractionEvent
            .Subscribe(OnCardInteraction)
            .AddTo(card.Transform);
    }

    public void TryMatching(ICardController card, ICardController withAnother)
    {
        if (!card.DoesMatch(withAnother))
            return;
        
        //card.Destroy();
        //withAnother.Destroy();
    }

    private void OnCardInteraction(CardInteractionEvent withEvent)
    {
        var card = withEvent.card;
        
        switch (withEvent.type)
        {
            case CardInteractionEventType.Pick:

                ToggleSlotHighlight(true, s => card.SlotMask.Contains(s.Type));
                
                break;
            case CardInteractionEventType.Drop:

                ToggleSlotHighlight(false);
                
                var targetedSlot = PlaySlots
                    .FirstOrDefault(s => s.DoesContain(card.Transform.position));
                
                if (targetedSlot == null)
                    return;

                if (targetedSlot.DoesAdmit(card))
                    targetedSlot.Take(card);
                else if (targetedSlot.DoesMatch(card))
                    TryMatching(card, targetedSlot.Head);

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void ToggleSlotHighlight(bool on, Func<ICardSlotController, bool> where = null)
    {
        var slotSelection = where != null ? PlaySlots.Where(where) : PlaySlots; 
        
        foreach (var slot in slotSelection)
            slot.ToggleHighlight(on);
    }*/
}