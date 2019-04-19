using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using UniRx;

public interface IBoardController
{
    List<ICardSlotController> PlaySlots { get; }
    List<ICardSlotController> StashSlots { get; }
    ICardSlotController PlayerSlot { get; }
    
    List<ICardSlotController> DropSlots { get; }
}

public class BoardController : IBoardController, IInitializable, IDisposable
{
    private readonly CardSlotController.Factory cardSlotControllerFactory;
    private readonly IBoard model;
    private readonly IBoardView view;
    private readonly DeckController deckController;
    private readonly GameSettings settings;
    private readonly CompositeDisposable perGameDisposables = new CompositeDisposable();
    private readonly Dictionary<ICardController, ICardSlotController> cardLocationMap 
        = new Dictionary<ICardController, ICardSlotController>();

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
        
        PlaySlots = view.SlotViews
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

        DropSlots = PlaySlots;
    }
    
    public List<ICardSlotController> PlaySlots { get; }
    public List<ICardSlotController> StashSlots { get; }
    public ICardSlotController PlayerSlot { get; }
    public List<ICardSlotController> DropSlots { get; }

    public void Initialize()
    {
        PlaySlots.ForEach(cardSlotController => Deal(cardSlotController, settings.MaxCardCountPerPlaySlot));
    }

    public void Dispose()
    {
        perGameDisposables.Dispose();
    }

    private void Deal(ICardSlotController onSlot, int count)
    {
        for (var i = 0; i < count; i++)
        {
            if (!onSlot.DoesAcceptNewGuests)
                break;
            
            var card = deckController.Draw();
            if (card == null)
                break;

            Lodge(card, onSlot);
            
            view.Parent(card.Transform);

            card.InteractionEvent
                .Subscribe(OnCardInteraction);
        }
    }

    private bool Lodge(ICardController card, ICardSlotController inSlot)
    {
        var canLodge = inSlot.Lodge(card);
        if (canLodge)
        {
            if (cardLocationMap.ContainsKey(card))
                cardLocationMap[card].Release(card);

            cardLocationMap[card] = inSlot;
        }

        return canLodge;
    }

    private void OnCardInteraction(CardInteractionEvent withEvent)
    {
        switch (withEvent.type)
        {
            case CardInteractionEventType.Pick:
                break;
            case CardInteractionEventType.Drop:

                var targetedSlot = DropSlots
                    .FirstOrDefault(s => s.DoesContain(withEvent.card.Transform.position));
                
                if (targetedSlot == null /* OR Slot does not accept Card*/)
                    return;

                Lodge(withEvent.card, targetedSlot);

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}