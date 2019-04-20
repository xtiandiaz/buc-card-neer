using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using UniRx;

public interface IBoardController
{
    List<ICardSlotController> ServiceSlots { get; }
    List<ICardSlotController> StashSlots { get; }
    ICardSlotController PlayerSlot { get; }
    List<ICardSlotController> PlaySlots { get; }

    void Deal(ICardSlotController onSlot, int count);
    void TryMatching(ICardController card, ICardController withAnother);
}

public class BoardController : IBoardController, IInitializable, IDisposable
{
    private readonly CardSlotController.Factory cardSlotControllerFactory;
    private readonly IBoard model;
    private readonly IBoardView view;
    private readonly DeckController deckController;
    private readonly GameSettings settings;
    private readonly CompositeDisposable perGameDisposables = new CompositeDisposable();

    private IDisposable moveSubscription;

    private BoardController(
        Board.Factory boardFactory,
        IBoardView view,
        DeckController.Factory deckControllerFactory,
        CardSlotController.Factory cardSlotControllerFactory,
        GameSettings settings
        )
    {
        model = boardFactory.Create();
        
        this.view = view;
        this.cardSlotControllerFactory = cardSlotControllerFactory;
        this.settings = settings;

        deckController = deckControllerFactory.Create();
        
        ServiceSlots = view.SlotViews
            .Where(s => s.Type == CardSlotType.Play)
            .Select(slotView => (ICardSlotController) cardSlotControllerFactory.Create(slotView))
            .ToList();
        
        StashSlots = view.SlotViews
            .Where(s => s.Type == CardSlotType.Stash)
            .Select(slotView => (ICardSlotController) cardSlotControllerFactory.Create(slotView))
            .ToList();

        PlayerSlot = view.SlotViews
            .Where(s => s.Type == CardSlotType.Player)
            .Select(slotView => (ICardSlotController) cardSlotControllerFactory.Create(slotView))
            .First();

        PlaySlots = ServiceSlots.ConvertAll(s => s);
        PlaySlots.AddRange(StashSlots);
    }
    
    public List<ICardSlotController> ServiceSlots { get; }
    public List<ICardSlotController> StashSlots { get; }
    public ICardSlotController PlayerSlot { get; }
    public List<ICardSlotController> PlaySlots { get; }

    public void Initialize()
    {
        ServiceSlots.ForEach(cardSlotController => Deal(cardSlotController, settings.MaxCardCountPerPlaySlot));
    }

    public void Dispose()
    {
        perGameDisposables.Dispose();
    }

    public void Deal(ICardSlotController onSlot, int count)
    {
        for (var i = 0; i < count; i++)
        {
            if (!onSlot.CanBeDealtOn)
                break;
            
            var card = deckController.Draw();
            if (card == null)
                break;

            onSlot.Take(card);
            
            view.Parent(card.Transform);

            card.InteractionEvent
                .Subscribe(OnCardInteraction)
                .AddTo(card.Transform);
        }
    }

    public void TryMatching(ICardController card, ICardController withAnother)
    {
        if (!card.DoesMatch(withAnother))
            return;
        
        card.Destroy();
        withAnother.Destroy();
    }

    private void OnCardInteraction(CardInteractionEvent withEvent)
    {
        var card = withEvent.card;
        
        switch (withEvent.type)
        {
            case CardInteractionEventType.Pick:

                ToggleSlotHighlight(true, s => s.DoesAdmit(card) || s.DoesMatch(card));
                
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
    }
}