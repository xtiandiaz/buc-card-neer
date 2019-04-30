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
    
    [SerializeField] private SortingGroup sortingGroup;
    [SerializeField] private SpriteRenderer frontFaceRenderer;
    [SerializeField] private SpriteRenderer backFaceRenderer;
    [SerializeField] private DraggingManager draggingManager;
    
    private GameSettings settings;
    private Transform thisTransform;
    private Tween positionTween;
    private Sequence flipSequence;

    public Sprite FrontFace
    {
        set => frontFaceRenderer.sprite = value;
    }
    
    public Sprite BackFace
    {
        set => backFaceRenderer.sprite = value;
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
        sortingGroup.enabled = true;
        sortingGroup.sortingOrder = settings.FloatingCardSortingOrder;

        positionTween?.Kill();
    }

    public void OnDropped()
    {
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
        var destEulerAngles = new Vector3(0, to == CardFace.Back ? 180f : 0, 0);

        if (!animated)
        {
            thisTransform.eulerAngles = destEulerAngles;
            return;
        }            
        
        flipSequence?.Kill();

        var tweenDuration = (float) settings.CardFlipDuration.TotalSeconds;
        
        flipSequence = DOTween.Sequence();
        flipSequence.Append(transform.DORotate(destEulerAngles, tweenDuration));
        
        //TODO: Also lift the card while rotating
        
        flipSequence.SetEase(Ease.InOutQuint);
    }

    public void ToggleDragging(bool on)
    {
        draggingManager.ToggleDragging(on);
    }
    
    public void Destroy()
    {
        Destroy(gameObject);
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