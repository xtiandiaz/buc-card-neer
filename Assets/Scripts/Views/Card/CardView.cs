using System;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

public interface ICardView
{
    Sprite FrontFace { set; }
    Sprite BackFace { set; }
    Vector3 LocalPosition { get; set; }
    IObservable<Unit> DragStarted { get; }
    IObservable<Vector3> Dragged { get; }
    IObservable<Vector3> DragEnded { get; }
    
    void OnPicked();
    void OnDropped();
    void Flip(CardFace to, bool animated);
    void ToggleVisibility(bool on);
    void ToggleDragging(bool on);
    void SetParent(Transform asTransform);
    void SetLocalPosition(Vector3 to, float duringSeconds);
    void Destroy();
}

public class CardView : MonoBehaviour, ICardView
{
    public class Factory : PlaceholderFactory<string, CardView>
    {
    }

    [Header("Rendering")]
    [SerializeField] private int textSortingOrder;
    [SerializeField] private MeshRenderer[] textRenderers;

    [SerializeField] private Transform contentWrapper;
    [SerializeField] private SortingGroup sortingGroup;
    [SerializeField] private CardFaceView frontFace;
    [SerializeField] private CardFaceView backFace;
    [SerializeField] private DraggingManager draggingManager;
    
    private GameSettings settings;
    private Transform thisTransform;
    private Tween positionTween, raiseTween;
    private Sequence flipSequence;

    public Sprite FrontFace
    {
        set => frontFace.Sprite = value;
    }
    
    public Sprite BackFace
    {
        set => backFace.Sprite = value;
    }
    
    public IObservable<Unit> DragStarted => draggingManager.DragStarted;
    public IObservable<Vector3> Dragged => draggingManager.Dragged;
    public IObservable<Vector3> DragEnded => draggingManager.DragEnded;

    public Vector3 LocalPosition
    {
        get => thisTransform.localPosition;
        set
        {
            positionTween?.Kill();
            thisTransform.localPosition = value;
        }
    }

    [Inject]
    private void Construct(GameSettings settings)
    {
        this.settings = settings;
        
        thisTransform = transform;

        sortingGroup.enabled = false;
        
        foreach (var renderer in textRenderers)
        {
            renderer.sortingLayerName = settings.CardSortingLayerName;
            renderer.sortingOrder = textSortingOrder;
        }
    }

    public void OnPicked()
    {
        positionTween?.Kill();
        
        raiseTween?.Kill();
        raiseTween = Raise(settings.CardSize.x, 0.2f)
            .SetEase(Ease.OutQuart);
        
        sortingGroup.enabled = true;
        sortingGroup.sortingOrder = settings.FloatingCardSortingOrder;
    }

    public void OnDropped()
    {
        raiseTween?.Kill();
        raiseTween = Raise(0, 0.2f)
            .SetEase(Ease.OutQuart);
        
        sortingGroup.enabled = false;
    }

    public void ToggleVisibility(bool on)
    {
        gameObject.SetActive(on);
    }

    public void SetParent(Transform asTransform)
    {
        thisTransform.SetParent(asTransform, true);
    }
    
    public void SetLocalPosition(Vector3 to, float duringSeconds)
    {
        Move(to, duringSeconds);
    }

    public void Flip(CardFace to, bool animated)
    {
        flipSequence?.Kill();
        raiseTween?.Kill();
        
        var destEulerAngles = Vector3.up * (to == CardFace.Back ? 180f : 0);

        void ToggleFaceVisibility()
        {
            frontFace.ToggleVisibility(to == CardFace.Front);
            backFace.ToggleVisibility(to == CardFace.Back);
        }

        if (!animated)
        {
            thisTransform.eulerAngles = destEulerAngles;
            ToggleFaceVisibility();
            
            return;
        }            
        
        var halfTweenDuration = (float) settings.CardFlipDuration.TotalSeconds;
        
        flipSequence = DOTween.Sequence();
        flipSequence.Append(
            contentWrapper.DORotate(Vector3.up * 90f, halfTweenDuration)
                .OnComplete(() => ToggleFaceVisibility())
                .SetEase(Ease.InQuart));
        
        flipSequence.Join(
            Raise(settings.CardSize.x, halfTweenDuration)
                .SetEase(Ease.InQuart));
        
        flipSequence.Append(
            contentWrapper.DORotate(destEulerAngles, halfTweenDuration).SetEase(Ease.OutQuart));
        
        flipSequence.Join(
            Raise(0, halfTweenDuration)
                .SetEase(Ease.OutQuart));
    }

    public void ToggleDragging(bool on)
    {
        draggingManager.ToggleDragging(on);
    }
    
    public void Destroy()
    {
        Destroy(gameObject);
    }

    private Tween Raise(float toDepth, float andAnimateDuringSeconds)
    {
        return contentWrapper.DOLocalMoveZ(toDepth, andAnimateDuringSeconds);
    }

    private void Move(Vector3 toPosition, float duringSeconds, TweenCallback andDoOncomplete = null)
    {
        positionTween?.Kill();
        positionTween = thisTransform.DOLocalMove(toPosition, duringSeconds)
            .SetEase(Ease.OutQuint);

        if (andDoOncomplete != null)
            positionTween.OnComplete(andDoOncomplete);
    }
}