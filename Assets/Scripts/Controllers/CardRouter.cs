using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

public interface ICardRouter : IDisposable, IInitializable
{
    void Register(ISlot slot);
}

public class CardRouter : ICardRouter
{
    private readonly Subject<(ICard, ISlot)> picking = new Subject<(ICard, ISlot)>();
    private readonly Subject<(ICard, ISlot, Vector3)> routing = new Subject<(ICard, ISlot, Vector3)>();
    private readonly List<ISlot> slots = new List<ISlot>();
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    
    private readonly ICardDeferrer deferrer;
    private readonly ICardDismisser dismisser;
    private readonly ICardMatcher matcher;
    private readonly ICardHost host;
    private readonly IAudioManager audioManager;

    private CardRouter(
        ICardDeferrer deferrer,
        ICardDismisser dismisser,
        ICardMatcher matcher,
        ICardHost host,
        IAudioManager audioManager
        )
    {
        this.deferrer = deferrer;
        this.dismisser = dismisser;
        this.matcher = matcher;
        this.host = host;
        this.audioManager = audioManager;
    }

    public void Initialize()
    {
        disposables.Add(picking
            .Do(_ => audioManager.Play(AudioEventKey.UIDragGrab))
            .Subscribe(cardFromSlot =>
            {
                var (card, fromSlot) = cardFromSlot;
                
                foreach (var slot in slots)
                {
                    if (slot == fromSlot)
                        continue;

                    slot.ToggleHighlight(CanRoute( fromSlot, slot));
                }
            }));
        
        disposables.Add(routing
            .Select(cardFromSlotAtPosition =>
            {
                var (card, fromSlot, dropPos) = cardFromSlotAtPosition;

                return new
                {
                    Card = card,
                    SourceSlot = fromSlot,
                    DestinationSlot = slots.FirstOrDefault(slot => slot != fromSlot && slot.DoesContain(dropPos))
                };
            })
            .SelectMany(route =>
            {
                if (route.DestinationSlot != null) 
                    return Route(route.Card, route.SourceSlot, route.DestinationSlot);

                if (dismisser.CanDismiss(route.SourceSlot))
                    return dismisser.Dismiss(route.SourceSlot)
                        .DoOnSubscribe(() => audioManager.Play(AudioEventKey.CardBridgeDismiss));
                
                return Observable.ReturnUnit();
            })
            .Subscribe());
    }
    
    public void Register(ISlot slot)
    {
        slots.Add(slot);

        disposables.Add(slot.WhenPressed
            .Select(_ => slot.Peek())
            .Where(card => card != null && !slot.IsLocked)
            .Take(1)
            .Do(card =>
            {
                card.Pick();
                picking.OnNext((card, slot));
            })
            .ContinueWith(card => slot.WhenReleased
                .Take(1)
                .Do(_ => ToggleSlotHighlights(false))
                .ContinueWith(card.Drop()
                    .DoOnSubscribe(() => audioManager.Play(AudioEventKey.UIDragCancel))))
            .RepeatSafe()
            .Subscribe());

        disposables.Add(slot.WhenDraggingStarted
            .Take(1)
            .Select(_ => slot.Peek())
            .Where(card => card != null && !slot.IsLocked)
            .ContinueWith(pickedCard => slot.WhenDragged
                .TakeUntil(slot.WhenDraggingStopped)
                .Do(pickedCard.Drag)
                .Last()
                .Select(_ =>
                {
                    var slotWorldPos = slot.Position;
                        
                    return new 
                    {
                        Card = pickedCard,
                        Position = new Vector3(
                            slotWorldPos.x + pickedCard.LocalPosition.x,
                            slotWorldPos.y + pickedCard.LocalPosition.y)
                    };
                }))
            .RepeatSafe()
            .Subscribe(droppedCardAtPosition => 
                routing.OnNext((droppedCardAtPosition.Card, slot, droppedCardAtPosition.Position))));
    }

    public void Dispose()
    {
        picking.Dispose();
        routing.Dispose();
        
        disposables?.Dispose();
    }

    private bool CanRoute(ISlot fromSource, ISlot intoDestination)
    {
        return deferrer.CanDefer(fromSource, intoDestination) || 
               matcher.CanMatch(fromSource, intoDestination) ||
               host.CanLodge(fromSource, intoDestination);
    }

    private IObservable<Unit> Route(ICard card, ISlot fromSource, ISlot intoDestination)
    {
        return Observable.Create<Unit>(observer =>
        {
            if (deferrer.CanDefer(fromSource, intoDestination))
            {
                return deferrer.Defer(fromSource, fromSource)
                    .Subscribe(observer);
            }
            
            if (matcher.CanMatch(fromSource, intoDestination))
            {
                return matcher.Match(fromSource, intoDestination)
                    .Subscribe(observer);
            }
            
            if (host.CanLodge(fromSource, intoDestination))
            {
                return host.Lodge(fromSource, intoDestination)
                    .Subscribe(observer);
            }

            return card.Drop()
                .DoOnSubscribe(() => audioManager.Play(AudioEventKey.UIDragCancel))
                .Subscribe(observer);
        });
    }

    private void ToggleSlotHighlights(bool toValue)
    {
        foreach (var slot in slots)
            slot.ToggleHighlight(toValue);
    }
}