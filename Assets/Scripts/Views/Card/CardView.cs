using System;
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
    CardAnimationSettings AnimationSettings { get; }

    void Lodge(Vector3 atLocalPosition);
    void OnPicked();
    void OnDropped();
    void Flip(CardFace to, bool animated);
    void Move(Vector3 toPosition);
    IObservable<Unit> MoveAsObservable(Vector3 toPosition);
    void ToggleVisibility(bool on);
    void SetParent(Transform asTransform);
    void Fade(float toAlphaValue);
    void Tint(Color withColor, float byFactor);
    void Fog(Color withColor, float byFactor);
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
    
    private IDraggingManager draggingManager;
    private ICardAnimator animator;
    private ICardShader shader;
    private BoardLayoutSettings layoutSettings;
    private Vector3 lastLodgingPosition;

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
    public CardAnimationSettings AnimationSettings { get; private set; }

    public Vector3 LocalPosition
    {
        get => transform.localPosition;
        set
        {
            animator.Kill(CardAnimationType.Move);
            transform.localPosition = value;
        }
    }

    [Inject]
    private void Construct(
        CardAnimationSettings animationSettings, 
        BoardLayoutSettings layoutSettings,
        IWorldPointProvider worldPointProvider
        )
    {
        AnimationSettings = animationSettings;
        this.layoutSettings = layoutSettings;
        
        draggingManager = GetComponent<IDraggingManager>() ?? gameObject.AddComponent<DraggingManager>();
        draggingManager.Initialize(worldPointProvider);
        
        animator = GetComponent<ICardAnimator>() ?? gameObject.AddComponent<CardAnimator>();
        animator.Initialize(animationSettings, contentWrapper);

        shader = GetComponent<ICardShader>();

        sortingGroup.enabled = false;

        foreach (var renderer in textRenderers)
        {
            renderer.sortingLayerName = layoutSettings.CardSortingLayerName;
            renderer.sortingOrder = textSortingOrder;
        }
    }

    public void Lodge(Vector3 atLocalPosition)
    {
        LocalPosition = lastLodgingPosition = atLocalPosition;
    }

    public void OnPicked()
    {
        animator.Kill(CardAnimationType.Move);
        animator.Lift();

        sortingGroup.enabled = true;
        sortingGroup.sortingOrder = layoutSettings.FloatingCardSortingOrder;
    }

    public void OnDropped()
    {
        animator.Drop();

        sortingGroup.enabled = false;
    }

    public void ToggleVisibility(bool on)
    {
        gameObject.SetActive(on);
    }

    public void Fade(float toAlphaValue)
    {
        shader.Fade(toAlphaValue);
    }

    public void Tint(Color withColor, float byFactor)
    {
        shader.Tint(withColor, byFactor);
    }
    
    public void Fog(Color withColor, float byFactor)
    {
        shader.Fog(withColor, byFactor);
    }

    public void Flip(CardFace to, bool animated)
    {
        animator.Flip(to, animated, () =>
        {
            frontFace.ToggleVisibility(to == CardFace.Front);
            backFace.ToggleVisibility(to == CardFace.Back);
        });
    }

    public void Move(Vector3 toPosition)
    {
        animator.Move(toPosition, 0.5f);
    }
    
    public IObservable<Unit> MoveAsObservable(Vector3 toPosition)
    {
        return animator.MoveAsObservable(toPosition, 0.5f);
    }
    
    public void SetParent(Transform asTransform)
    {
        transform.SetParent(asTransform, true);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}