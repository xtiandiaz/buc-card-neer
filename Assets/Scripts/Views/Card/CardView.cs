using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Zenject;
using UniRx;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public interface ICardView
{
    void Arrange(Vector3 atLocalPosition, int withSortingOrder);
    
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
    
    private MeshRenderer textMeshRenderer;
    private Tween locationTween;
    private Tween flipTween;
    private Sequence disposeSequence;
    private Sequence flipSequence;
    private Transform thisTransform;

    public Vector2 Size => frontFace.size;

    [Inject]
    private void Construct(
        GamePalette palette, 
        GameSettings settings, 
        IBoardCamera boardCamera 
        )
    {
        this.palette = palette;
        this.settings = settings;
        this.boardCamera = boardCamera;
        
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
    }

    protected virtual void Start()
    {
        flipTween = transform.DORotate(
                Vector3.zero,
                settings.MoveDurationInSeconds * 0.75f)
            .SetEase(Ease.InOutQuint)
            .OnComplete(() => backFace.enabled = false);
    }

    public void Arrange(Vector3 atLocalPosition, int withSortingOrder)
    {
        thisTransform.localPosition = atLocalPosition;
        sortingGroup.sortingOrder = withSortingOrder;
        //sortingGroup.enabled = false;
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
        Destroy(gameObject);
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
}
