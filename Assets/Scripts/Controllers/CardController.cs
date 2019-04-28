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
    SlotType SlotMask { get; }

    void Initialize();
    //void Arrange(Vector3 atLocalPosition, int andIndexInStack, int withStackCount, CardStackLayout andLayout);
    bool DoesMatch(ICardController other);
}

public class CardController : ICardController, IDisposable
{
    public class Factory : PlaceholderFactory<ICard, ICardView, CardController>
    {
    }
    
    private readonly ICard model;
    private readonly ICardView view;
    private readonly BoardCamera boardCamera;
    private readonly ObservableEventTrigger eventTrigger;
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    private readonly GameSettings settings;

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
    }
    
    public SlotType SlotMask => model.SlotMask;

    public void Initialize()
    {
        var draggingManager = view.AddComponent<DraggingManager>();
        draggingManager.Initialize(boardCamera);
        
        disposables.Add(draggingManager.DragStarted.Subscribe(_ => model.Pick()));
        disposables.Add(draggingManager.Dragged.Subscribe(worldPositionDelta => view.Position += worldPositionDelta));
        disposables.Add(draggingManager.DragEnded.Subscribe(_ => model.Drop()));
        
        draggingManager.ToggleDragging(true);
        
        disposables.Add(model.Picked.Subscribe(_ => view.OnPicked()));
        disposables.Add(model.Dropped.Subscribe(_ =>
        {
            view.OnDropped();
            view.Set(model.Position, settings.CardReturnDuration);

        }));

        disposables.Add(model.PositionChanged.Subscribe(position => view.Set(position, settings.CardMoveDuration)));
    }

    public virtual bool DoesMatch(ICardController other)
    {
        return other != null /*&& (model.Type & other.InteractionMask) != 0*/;
    }

    public void Dispose()
    {
        disposables?.Dispose();
        view.Destroy();
    }
}