using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Zenject;
using UniRx;
using Random = UnityEngine.Random;

public interface ICardView
{
    Vector3 LocalPosition { get; set; }
    
    void Destroy();
}

public abstract class CardView : MonoBehaviour, ICardView
{
    [SerializeField] protected SpriteRenderer frontFace;
    protected ICard card;
    protected GamePalette palette;
    protected GameSettings settings;
    protected IBoardCamera boardCamera;
    
    [SerializeField] private Renderer[] bodyRenderersForSorting;
    [SerializeField] private Renderer[] textRenderersForSorting;
    private MeshRenderer textMeshRenderer;
    private Tween locationTween;
    private Tween flipTween;
    private Sequence disposeSequence;
    private Sequence flipSequence;
    private Transform thisTransform;

    public Vector2 Size => frontFace.size;
    public Vector3 LocalPosition
    {
        get => thisTransform.localPosition;
        set => thisTransform.localPosition = value;
    }

    protected virtual string DefaultSortingLayer => settings.CardDefaultSortingLayerName;
    protected virtual string DefaultTextSortingLayer => settings.CardTextDefaultSortingLayerName;
    private string FirstOverlaySortingLayer => settings.CardFirstOverlaySortingLayerName;
    private string FirstOverlayTextSortingLayer => settings.CardTextFirstOverlaySortingLayerName;
    private string SecondOverlaySortingLayer => settings.CardSecondOverlaySortingLayerName;
    private string SecondOverlayTextSortingLayer => settings.CardTextSecondOverlaySortingLayerName;

    [Inject]
    private void Construct(
        ICard card, 
        GamePalette palette, 
        GameSettings settings, 
        IBoardCamera boardCamera 
        )
    {
        this.card = card;
        this.palette = palette;
        this.settings = settings;
        this.boardCamera = boardCamera;
        
        thisTransform = transform;

        Initialize();
    }
    
    protected abstract void Initialize();

    protected virtual void Awake()
    {
        SetToFirstOverlaySortingLayers();

        var cameraPos = boardCamera.Position;
        var viewRect = boardCamera.GetFrustumRect(transform.position.z);
        
        transform.position = new Vector3(
            cameraPos.x, 
            cameraPos.y + (viewRect.height + Size.y) * 0.5f, 
            cameraPos.z * 0.5f);
        
        transform.rotation = Quaternion.Euler(0, 180f, 0);
    }

    protected virtual void Start()
    {
        var introDelay = TimeSpan.FromSeconds(
            Random.Range(settings.MoveDurationInSeconds * 0.25f, settings.MoveDurationInSeconds * 0.5f));

        flipTween = transform.DORotate(
                Vector3.zero,
                settings.MoveDurationInSeconds * 0.75f)
            .SetDelay((float)introDelay.TotalSeconds)
            .SetEase(Ease.InOutQuint)
            .OnComplete(SetToDefaultSortingLayers);
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
            .OnStart(SetToSecondOverlaySortingLayers)
            .SetEase(Ease.InOutQuint));

        disposeSequence.OnComplete(() => Destroy(gameObject));
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
    
    protected Color GetTypeColor()
    {
        switch (card.Type)
        {
            case CardType.Player:
                return Color.white;
            case CardType.Health:
                return palette.Health;
            case CardType.Stamina:
                return palette.Stamina;
            case CardType.Defense:
                return palette.Defense;
            case CardType.Ability:
                return palette.Ability1;
            case CardType.Baddie:
                return palette.Baddie;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void SetPosition(Coordinates forCoordinates)
    {
        locationTween?.Kill();
        locationTween = transform.DOMove(
                new Vector3(
                    forCoordinates.x,
                    forCoordinates.y, 0) * settings.DisplacementUnit,
                settings.MoveDurationInSeconds)
            .SetEase(Ease.OutQuint);
    }

    private void SetToFirstOverlaySortingLayers()
    {
        SetSortingLayer(FirstOverlaySortingLayer, bodyRenderersForSorting);
        SetSortingLayer(FirstOverlayTextSortingLayer, textRenderersForSorting);
    }
    
    private void SetToSecondOverlaySortingLayers()
    {
        SetSortingLayer(SecondOverlaySortingLayer, bodyRenderersForSorting);
        SetSortingLayer(SecondOverlayTextSortingLayer, textRenderersForSorting);
    }
    
    private void SetToDefaultSortingLayers()
    {
        SetSortingLayer(DefaultSortingLayer, bodyRenderersForSorting);
        SetSortingLayer(DefaultTextSortingLayer, textRenderersForSorting);
    }

    private void SetSortingLayer(string named, IEnumerable<Renderer> forRenderers)
    {
        var sortingLayerId = SortingLayer.NameToID(named);
        
        foreach (var sortingRenderer in forRenderers)
        {
            sortingRenderer.sortingLayerID = sortingLayerId;
        }
    }
}
