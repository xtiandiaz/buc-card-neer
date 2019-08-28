using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

public interface IRoutingController : IDisposable, IInitializable
{
    IObservable<Unit> WhenCardPicked { get; }
    IObservable<Unit> WhenCardDropped { get; }
}

public class RoutingController : IRoutingController
{
    private readonly Subject<(ICard, ISlot)> picking = new Subject<(ICard, ISlot)>();
    private readonly Subject<Unit> dropping = new Subject<Unit>();
    private readonly Subject<(ICard, ISlot, Vector3)> routing = new Subject<(ICard, ISlot, Vector3)>();
    private readonly List<ISlot> slots = new List<ISlot>();
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private readonly IWorldPointProvider worldPointProvider;
    private readonly IBoardModel boardModel;
    private readonly IBoard board;
    private readonly IForwardingController forwarder;
    private readonly IDefermentController deferrer;
    private readonly IDismissalController dismisser;
    private readonly IMatchingController matcher;
    private readonly ILodgingController host;

    private RoutingController(
        IWorldPointProvider worldPointProvider,
        IBoardModel boardModel,
        IBoard board,
        IForwardingController forwarder,
        IDefermentController deferrer,
        IDismissalController dismisser,
        IMatchingController matcher,
        ILodgingController host
    )
    {
        this.worldPointProvider = worldPointProvider;
        this.boardModel = boardModel;
        this.board = board;
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
        foreach (var slot in board.Sea.Slots)
            Register(slot);
        
        Register(board.Ship.Helm);
        Register(board.Ship.Plank);
        Register(board.Ship.Mount);
        Register(board.Ship.Storage);

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
            .Where(_ => slot.Peek() != null && !slot.IsLocked)
            .Take(1)
            .Select(pickingScreenPos => 
            {
                var card = slot.Peek();
                var pickingPos = worldPointProvider.GetWorldPoint(pickingScreenPos, boardModel.FloatingCardDepth);
                var pickingOffset = slot.Position - worldPointProvider.GetWorldPoint(pickingScreenPos, slot.Position.z);
                
                card.Pick(pickingPos + pickingOffset);
                picking.OnNext((card, slot));
                
                return new {Card = card, PickingOffset = pickingOffset};
            })
            .ContinueWith(pickedCardWithOffset => slot.WhenUnpressed
                .Take(1)
                .Merge(slot.WhenDraggingStarted
                    .Take(1)
                    .ContinueWith(slot.WhenDragged
                        .TakeUntil(slot.WhenDraggingStopped)
                        .Do(draggingScreenPos => 
                            pickedCardWithOffset.Card.Drag(
                                worldPointProvider.GetWorldPoint(draggingScreenPos, boardModel.FloatingCardDepth) + pickedCardWithOffset.PickingOffset))
                        .AsSingleUnitObservable()))
                .Do(_ => 
                {
                    ToggleSlotHighlights(false);
                        
                    var slotWorldPos = slot.Position;

                    routing.OnNext((
                        pickedCardWithOffset.Card, 
                        slot, 
                        new Vector3(
                            slotWorldPos.x + pickedCardWithOffset.Card.LocalPosition.x,
                            slotWorldPos.y + pickedCardWithOffset.Card.LocalPosition.y)));
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
            if (deferrer.CanDefer(fromSource, intoDestination))
            {
                return deferrer.Defer(fromSource, intoDestination)
                    .Subscribe(observer);
            }
            
            if (forwarder.CanForward(card, intoDestination))
            {
                return forwarder.Forward(card, intoDestination)
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