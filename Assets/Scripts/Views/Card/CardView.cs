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
    void Arrange(Vector3 atLocalPosition, int withStackIndex, int andSortingOrder);
    
    void Destroy();
}

public abstract class CardView : MonoBehaviour, ICardView
{
    [SerializeField] protected SpriteRenderer frontFace;
    [SerializeField] protected SpriteRenderer backFace;
    [SerializeField] protected SortingGroup sortingGroup;
    protected GamePalette palette;
    protected GameSettings settings;
    protected IBoardCamera boardCamera;
    
    private readonly ReactiveProperty<int?> stackIndex = new ReactiveProperty<int?>();
    private readonly CompositeDisposable interactionEventDisposables = new CompositeDisposable();
    private ObservableEventTrigger eventTrigger;
    private MeshRenderer textMeshRenderer;
    private Tween locationTween;
    private Tween flipTween;
    private Sequence disposeSequence;
    private Sequence flipSequence;
    private Transform thisTransform;
    private Vector3 defaultLocalPosition;
    private int defaultSortingOrder;

    public Vector2 Size => frontFace.size;

    [Inject]
    private void Construct(
        GamePalette palette, 
        GameSettings settings, 
        IBoardCamera boardCamera,
        UserInteractionListener interactionListener
        )
    {
        this.palette = palette;
        this.settings = settings;
        this.boardCamera = boardCamera;

        eventTrigger = gameObject.AddComponent<ObservableEventTrigger>();
        
        thisTransform = transform;

        Initialize();
    }
    
    protected abstract void Initialize();

    protected virtual void Awake()
    {
        var cameraPos = boardCamera.Position;
        var viewRect = boardCamera.GetFrustumRect(transform.position.z);
        
        transform.position = new Vector3(
            cameraPos.x, 
            cameraPos.y + (viewRect.height + Size.y) * 0.5f, 
            cameraPos.z * 0.5f);
        
        transform.rotation = Quaternion.Euler(0, 180f, 0);

        stackIndex
            .Where(index => index.HasValue)
            .Subscribe(index =>
            {
                if (index == 0)
                    SubscribeToInteractionEvents();
                else
                    interactionEventDisposables.Clear();
            })
            .AddTo(this);
    }

    protected virtual void Start()
    {
        flipTween = transform.DORotate(
                Vector3.zero,
                settings.MoveDurationInSeconds * 0.75f)
            .SetEase(Ease.InOutQuint)
            .OnComplete(() =>
            {
                backFace.enabled = false;
            });
    }

    public void Arrange(Vector3 atLocalPosition, int withStackIndex, int andSortingOrder)
    {
        defaultLocalPosition = thisTransform.localPosition = atLocalPosition;
        defaultSortingOrder = sortingGroup.sortingOrder = andSortingOrder;
        stackIndex.Value = withStackIndex;
    }

    public void Flip()
    {
        flipSequence?.Kill();

        var tweenDuration = settings.MoveDurationInSeconds * 0.5f;
        
        flipSequence = DOTween.Sequence();
        flipSequence.Append(transform.DORotate(
                new Vector3(0, 180, 0),
                tweenDuration)
            .SetEase(Ease.InOutQuint));

        flipSequence.Join(transform.DOPunchPosition(Vector3.back * Size.x, tweenDuration)
                .SetEase(Ease.InOutQuint));
    }

    public void OnDispose()
    {
        flipTween?.Kill();
        locationTween?.Kill();
        flipSequence?.Kill();
        
        var cameraPos = boardCamera.Position;
        var viewRect = boardCamera.GetFrustumRect(transform.position.z);
        
        disposeSequence = DOTween.Sequence();
        disposeSequence.Append(transform.DOMove(
                new Vector3(
                    cameraPos.x,
                    cameraPos.y - (viewRect.height + Size.y) * 0.5f,
                    cameraPos.z * 0.5f),
                settings.MoveDurationInSeconds)
            .SetEase(Ease.InOutQuint));

        disposeSequence.Join(transform.DORotate(
            new Vector3(0, -180, 0),
            settings.MoveDurationInSeconds * 0.75f)
            .SetEase(Ease.InOutQuint));

        disposeSequence.OnComplete(() => Destroy(gameObject));
    }

    public void Destroy()
    {
        interactionEventDisposables.Dispose();
        
        Destroy(gameObject);
    }

    private void SubscribeToInteractionEvents()
    {
        interactionEventDisposables.Clear();
        
        var lastDragWorldPos = Vector3.zero;

        interactionEventDisposables.Add(
            eventTrigger
                .OnBeginDragAsObservable()
                .Subscribe(eventData =>
                {
                    lastDragWorldPos = boardCamera.GetWorldPosition(eventData.position);
                    sortingGroup.sortingOrder = settings.ActiveCardSortingOrder;

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
                .Subscribe(deltaWorldPos => thisTransform.localPosition += deltaWorldPos));

        interactionEventDisposables.Add(
            eventTrigger
                .OnEndDragAsObservable()
                .Subscribe(_ =>
                {
                    Move(defaultLocalPosition, settings.CardReturnDuration);
                    sortingGroup.sortingOrder = defaultSortingOrder;
                }));
    }

    private void Move(Vector3 toLocalPosition, TimeSpan during)
    {
        locationTween?.Kill();
        locationTween = transform.DOLocalMove(toLocalPosition, (float) during.TotalSeconds)
            .SetEase(Ease.OutQuint);
    }
}
