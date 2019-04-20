using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Zenject;
using UniRx;
using UniRx.Triggers;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public interface ICardView
{
    Transform Transform { get; }
    IObservable<CardInteractionEventType> InteractionEvent { get; }
    
    void Destroy();
    void SetParent(Transform parent);
    void Arrange(Vector3 atLocalPos, int andIndexInStack, int withStackCount, CardStackLayout andLayout);
}

public abstract class CardView : MonoBehaviour, ICardView
{
    [SerializeField] protected SpriteRenderer frontFace;
    [SerializeField] protected SpriteRenderer backFace;
    [SerializeField] protected SortingGroup sortingGroup;
    
    private readonly ReactiveProperty<int> siblingIndex = new ReactiveProperty<int>();
    private readonly CompositeDisposable interactionEventDisposables = new CompositeDisposable();
    private readonly Subject<CardInteractionEventType> interactionEvent = new Subject<CardInteractionEventType>();

    [SerializeField] private BoxCollider2D hitArea;
    private GameSettings settings;
    private IBoardCamera boardCamera;
    private ObservableEventTrigger eventTrigger;
    private MeshRenderer textMeshRenderer;
    private Tween locationTween;
    private Vector3 defaultLocalPosition;

    public Transform Transform { get; private set; }
    public IObservable<CardInteractionEventType> InteractionEvent => interactionEvent;
    
    private int DefaultSortingOrder => settings.MaxCardCountPerPlaySlot - siblingIndex.Value;
    
    [Inject]
    private void Construct(
        GameSettings settings, 
        IBoardCamera boardCamera
        )
    {
        this.settings = settings;
        this.boardCamera = boardCamera;

        eventTrigger = gameObject.AddComponent<ObservableEventTrigger>();
        Transform = transform;

        Initialize();
    }
    
    protected abstract void Initialize();

    private void Awake()
    {
        Transform.rotation = Quaternion.Euler(0, 180f, 0);
    }

    private void Start()
    {
        Transform.DORotate(
                Vector3.zero,
                settings.MoveDurationInSeconds * 0.75f)
            .SetEase(Ease.InOutQuint)
            .OnComplete(() =>
            {
                backFace.enabled = false;
            });
    }

    public void Arrange(Vector3 atLocalPos, int andIndexInStack, int withStackCount, CardStackLayout andLayout)
    {
        var sortingOrder = withStackCount - andIndexInStack - 1;
        var positionOffset = andLayout == CardStackLayout.Vertical
            ? Vector3.up * andIndexInStack * settings.CardOffsetInPile.y
            : Vector3.right * sortingOrder * settings.CardOffsetInPile.x;

        defaultLocalPosition =
            Transform.localPosition = atLocalPos + positionOffset;
        
        sortingGroup.sortingOrder = sortingOrder;

        if (andIndexInStack == 0)
            EnableUserInteraction();
        else
            DisableUserInteraction();
    }

    public void SetParent(Transform parent)
    {
        Transform.SetParent(parent, false);
    }

    public void Destroy()
    {
        interactionEventDisposables.Dispose();
        
        Destroy(gameObject);
    }

    private void EnableUserInteraction()
    {
        interactionEventDisposables.Clear();

        hitArea.enabled = true;
        
        var lastDragWorldPos = Vector3.zero;

        interactionEventDisposables.Add(
            eventTrigger
                .OnBeginDragAsObservable()
                .Subscribe(eventData =>
                {
                    interactionEvent.OnNext(CardInteractionEventType.Pick);
                    
                    lastDragWorldPos = boardCamera.GetWorldPosition(eventData.position);
                    sortingGroup.sortingOrder = settings.FloatingCardSortingOrder;

                    locationTween?.Kill();
                }));

        interactionEventDisposables.Add(
            eventTrigger
                .OnDragAsObservable()
                .TakeUntilDisable(this)
                .Select(eventData =>
                {
                    var worldPos = boardCamera.GetWorldPosition(eventData.position);
                    var delta = worldPos - lastDragWorldPos;

                    lastDragWorldPos = worldPos;

                    return delta;
                })
                .Subscribe(deltaWorldPos => Transform.localPosition += deltaWorldPos));

        interactionEventDisposables.Add(
            eventTrigger
                .OnEndDragAsObservable()
                .Subscribe(_ =>
                {
                    interactionEvent.OnNext(CardInteractionEventType.Drop);
                    
                    Move(
                        defaultLocalPosition, 
                        settings.CardReturnDuration, 
                        () => sortingGroup.sortingOrder = DefaultSortingOrder);
                }));
    }

    private void DisableUserInteraction()
    {
        interactionEventDisposables.Clear();
        hitArea.enabled = false;
    }

    private void Move(Vector3 toLocalPosition, TimeSpan during, TweenCallback andDoOncomplete = null)
    {
        locationTween?.Kill();
        locationTween = Transform.DOLocalMove(toLocalPosition, (float) during.TotalSeconds)
            .SetEase(Ease.OutQuint);

        if (andDoOncomplete != null)
            locationTween.OnComplete(andDoOncomplete);
    }
}
