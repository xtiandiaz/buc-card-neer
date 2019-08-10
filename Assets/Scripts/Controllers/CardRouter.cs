using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

public interface ICardRouter : IDisposable, IInitializable
{
    IObservable<Unit> WhenCardPicked { get; }
    IObservable<Unit> WhenCardDropped { get; }
}

public class CardRouter : ICardRouter
{
    private readonly Subject<(ICard, ISlot)> picking = new Subject<(ICard, ISlot)>();
    private readonly Subject<Unit> dropping = new Subject<Unit>();
    private readonly Subject<(ICard, ISlot, Vector3)> routing = new Subject<(ICard, ISlot, Vector3)>();
    private readonly List<ISlot> slots = new List<ISlot>();
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private readonly IShip ship;
    private readonly ISea sea;
    private readonly ICardForwarder forwarder;
    private readonly ICardDeferrer deferrer;
    private readonly ICardDismisser dismisser;
    private readonly ICardMatcher matcher;
    private readonly ICardHost host;

    private CardRouter(
        IShip ship,
        ISea sea,
        ICardForwarder forwarder,
        ICardDeferrer deferrer,
        ICardDismisser dismisser,
        ICardMatcher matcher,
        ICardHost host
    )
    {
        this.ship = ship;
        this.sea = sea;
        this.forwarder = forwarder;
        this.deferrer = deferrer;
        this.dismisser = dismisser;
        this.matcher = matcher;
        this.host = host;
    }

    public IObservable<Unit> WhenCardPicked => picking.AsUnitObservable();
    public IObservable<Unit> WhenCardDropped => dropping;

    public void Initialize()
    {
        foreach (var slot in sea.Slots)
            Register(slot);
        
        Register(ship.Helm);
        Register(ship.Plank);
        Register(ship.Mount);
        Register(ship.Storage);

        disposables.Add(picking
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
                    DestinationSlot = slots.FirstOrDefault(slot => slot.DoesContain(dropPos))
                };
            })
            .SelectMany(route =>
            {
                if (route.DestinationSlot != route.SourceSlot)
                {
                    if (dismisser.CanDismiss(route.SourceSlot))
                        return dismisser.Dismiss(route.SourceSlot);
                    
                    if (route.DestinationSlot != null)
                        return Route(route.Card, route.SourceSlot, route.DestinationSlot);
                }

                route.Card.Drop();
                dropping.OnNext(Unit.Default);

                return Observable.ReturnUnit();
            })
            .Subscribe());
    }

    public void Dispose()
    {
        picking.Dispose();
        dropping.Dispose();
        routing.Dispose();

        disposables.Dispose();
    }
    
    private void Register(ISlot slot)
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
            .ContinueWith(pickedCard => slot.WhenUnpressed
                .Take(1)
                .Merge(slot.WhenDraggingStarted
                    .Take(1)
                    .ContinueWith(slot.WhenDragged
                        .TakeUntil(slot.WhenDraggingStopped)
                        .Do(pickedCard.Drag)
                        .AsSingleUnitObservable()))
                .Do(_ => 
                {
                    ToggleSlotHighlights(false);
                        
                    var slotWorldPos = slot.Position;

                    routing.OnNext((
                        pickedCard, 
                        slot, 
                        new Vector3(
                            slotWorldPos.x + pickedCard.LocalPosition.x,
                            slotWorldPos.y + pickedCard.LocalPosition.y)));
                })
                .First())
                .RepeatSafe()
            .Subscribe());
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
            if (forwarder.CanForward(card, intoDestination))
            {
                return forwarder.Forward(card, intoDestination)
                    .Subscribe(observer);
            }
            
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

            return card.DropAsObservable()
                .DoOnSubscribe(() => dropping.OnNext(Unit.Default))
                .Subscribe(observer);
        });
    }

    private void ToggleSlotHighlights(bool toValue)
    {
        foreach (var slot in slots)
            slot.ToggleHighlight(toValue);
    }
}