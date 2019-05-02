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

    [Header("Rendering")] [SerializeField] private int textSortingOrder;
    [SerializeField] private MeshRenderer[] textRenderers;

    [SerializeField] private Transform contentWrapper;
    [SerializeField] private SortingGroup sortingGroup;
    [SerializeField] private CardFaceView frontFace;
    [SerializeField] private CardFaceView backFace;
    
    private IDraggingManager draggingManager;
    private ICardAnimator animator;
    private GameSettings settings;
    private Transform thisTransform;

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
            animator.Kill(CardAnimationType.Move);
            thisTransform.localPosition = value;
        }
    }

    [Inject]
    private void Construct(
        CardAnimationSettings animationSettings,
        GameSettings settings
        )
    {
        draggingManager = GetComponent<IDraggingManager>() ?? gameObject.AddComponent<DraggingManager>();
        
        animator = GetComponent<ICardAnimator>() ?? gameObject.AddComponent<CardAnimator>();
        animator.Initialize(animationSettings, contentWrapper);
        
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
        animator.Kill(CardAnimationType.Move);
        animator.Lift();

        sortingGroup.enabled = true;
        sortingGroup.sortingOrder = settings.FloatingCardSortingOrder;
    }

    public void OnDropped()
    {
        animator.PutDown();

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
        animator.Move(to, duringSeconds);
    }

    public void Flip(CardFace to, bool animated)
    {
        animator.Flip(to, animated, () =>
        {
            frontFace.ToggleVisibility(to == CardFace.Front);
            backFace.ToggleVisibility(to == CardFace.Back);
        });
    }

    public void ToggleDragging(bool on)
    {
        //draggingManager.ToggleDragging(on);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}