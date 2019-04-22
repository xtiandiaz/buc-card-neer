using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public enum CardInteractionEventType
{
    Pick,
    Drop
}

public struct CardInteractionEvent
{
    public CardInteractionEventType type;
    public ICardController card;

    public CardInteractionEvent(
        CardInteractionEventType type,
        ICardController card
    )
    {
        this.type = type;
        this.card = card;
    }
}

public interface ICardController
{
    Transform Transform { get; }
    bool IsDraggable { get; }
    CardType InteractionMask { get; }
    IObservable<CardInteractionEvent> InteractionEvent { get; }
    IObservable<Unit> Moved { get; }
    IObservable<Unit> Destroyed { get; }

    void Arrange(Vector3 atLocalPosition, int andIndexInStack, int withStackCount, CardStackLayout andLayout);
    bool DoesMatch(ICardController other);
    void OnMoved(ICardSlotController toSlot);
    void Destroy();
}

public class CardController : ICardController
{
    public class Factory : PlaceholderFactory<ICard, ICardView, CardController>
    {
        private readonly Card.Factory modelFactory;
        private readonly CardView.Factory viewFactory;
        private readonly ItemCardView.Factory resourceCardViewFactory;
        private readonly PirateCardView.Factory pirateCardViewFactory;
        private readonly MerchantCardView.Factory merchantCardViewFactory;

        private Factory(
            Card.Factory modelFactory,
            CardView.Factory viewFactory
            )
        {
            this.modelFactory = modelFactory;
            this.viewFactory = viewFactory;
        }
        
        public CardController Create(CardType withType)
        {
            return base.Create(modelFactory.Create(withType), viewFactory.Create(withType));
        }
        
        public CardController Create(ICard fromModel)
        {
            return base.Create(fromModel, viewFactory.Create(fromModel.Type));
        }
    }

    private readonly ICard model;
    private readonly ICardView view;
    private readonly BoardCamera boardCamera;
    private readonly GameSettings settings;
    private readonly ObservableEventTrigger eventTrigger;
    private readonly Subject<CardInteractionEventType> interactionEvent = new Subject<CardInteractionEventType>();
    private readonly CompositeDisposable interactionEventDisposables = new CompositeDisposable();
    
    private CardController(
        ICard model, 
        ICardView view, 
        BoardCamera boardCamera,
        GameSettings settings
        )
    {
        this.model = model;
        this.view = view;
        this.boardCamera = boardCamera;
        this.settings = settings;
        
        Moved = Observable.FromEvent(
            h => MovedEvent += h,
            h => MovedEvent -= h);
        
        Destroyed = Observable.FromEvent(
            h => DestroyedEvent += h,
            h => DestroyedEvent -= h);

        IsDraggable = (model.Type & CardType.Item) != 0;
        
        if (!IsDraggable)
            return;
        
        eventTrigger = view.Transform.gameObject.AddComponent<ObservableEventTrigger>();

        InteractionEvent = interactionEvent
            .Select(eventType => new CardInteractionEvent(eventType, this));
    }

    private event Action MovedEvent;
    private event Action DestroyedEvent;

    public Transform Transform => view.Transform;
    public bool IsDraggable { get; }
    public CardType InteractionMask => model.InteractionMask;
    public IObservable<CardInteractionEvent> InteractionEvent { get; }
    public IObservable<Unit> Moved { get; }
    public IObservable<Unit> Destroyed { get; }

    public void OnMoved(ICardSlotController toSlot)
    {
        MovedEvent?.Invoke();
    }
    
    public void Arrange(Vector3 atLocalPosition, int andIndexInStack, int withStackCount, CardStackLayout andLayout)
    {
        view.Arrange(atLocalPosition, andIndexInStack, withStackCount, andLayout);

        ToggleUserInteraction(andIndexInStack == 0 && IsDraggable);
    }

    public virtual bool DoesMatch(ICardController other)
    {
        return other != null && (model.Type & other.InteractionMask) != 0;
    }

    public void Destroy()
    {
        view.Destroy();

        DestroyedEvent?.Invoke();
    }
    
    private void ToggleUserInteraction(bool on)
    {
        view.HitArea.enabled = on;

        if (!on)
        {
            interactionEventDisposables.Clear();
            return;
        }
        
        if (interactionEventDisposables.Count > 0)
            return;

        var lastDragWorldPos = Vector3.zero;

        interactionEventDisposables.Add(
            eventTrigger
                .OnBeginDragAsObservable()
                .Subscribe(eventData =>
                {
                    interactionEvent.OnNext(CardInteractionEventType.Pick);

                    lastDragWorldPos = boardCamera.GetWorldPosition(eventData.position);
                    view.OnBeginDrag();
                }));

        interactionEventDisposables.Add(
            eventTrigger
                .OnDragAsObservable()
                .Select(eventData =>
                {
                    var worldPos = boardCamera.GetWorldPosition(eventData.position);
                    var delta = worldPos - lastDragWorldPos;

                    lastDragWorldPos = worldPos;

                    return delta;
                })
                .Subscribe(view.OnDrag));

        interactionEventDisposables.Add(
            eventTrigger
                .OnEndDragAsObservable()
                .Subscribe(_ =>
                {
                    interactionEvent.OnNext(CardInteractionEventType.Drop);
                    
                    view.OnDrop();
                }));
    }
}