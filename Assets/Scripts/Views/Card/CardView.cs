using System;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

public interface ICardView
{
    int Value { set; }
    Sprite FrontFace { set; }
    Sprite BackFace { set; }
    Vector3 Position { get; set; }
    int SortingOrder { set; } 
    
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

    [SerializeField] protected CardValue cardValue;
    [SerializeField] private Transform contentWrapper;
    [SerializeField] private SortingGroup sortingGroup;
    [SerializeField] private CardFaceView frontFace;
    [SerializeField] private CardFaceView backFace;
    
    private ICardAnimator animator;
    private ICardShader shader;
    private Vector3 lastLodgingPosition;
    private int sortingOrder;
    private BoardLayoutSettings layoutSettings;
    private CardAnimationSettings animationSettings;

    public int Value
    {
        set => cardValue.SetValue(value);
    }
    
    public Sprite FrontFace
    {
        set => frontFace.Sprite = value;
    }

    public Sprite BackFace
    {
        set => backFace.Sprite = value;
    }
    
    public Vector3 Position
    {
        get => transform.position;
        set
        {
            animator.Kill(CardAnimationType.Move);
            transform.position = value;
        }
    }

    public int SortingOrder
    {
        set
        {
            sortingGroup.sortingOrder = sortingOrder = value;

            var shouldToggleFaceContent = value >= -1;

            frontFace.ToggleContent(shouldToggleFaceContent);
            backFace.ToggleContent(shouldToggleFaceContent);
        }
    }

    [Inject]
    private void Construct(
        CardAnimationSettings animationSettings, 
        BoardLayoutSettings layoutSettings
        )
    {
        this.animationSettings = animationSettings;
        this.layoutSettings = layoutSettings;
        
        animator = GetComponent<ICardAnimator>() ?? gameObject.AddComponent<CardAnimator>();
        shader = GetComponent<ICardShader>();
    }

    private void Awake()
    {
        animator.Initialize(animationSettings, contentWrapper);
    }

    public void OnPicked()
    {
        animator.Kill(CardAnimationType.Move);
        //animator.Lift();

        sortingGroup.sortingOrder = layoutSettings.FloatingCardSortingOrder;
    }

    public void OnDropped()
    {
        //animator.Drop();

        sortingGroup.sortingOrder = sortingOrder;
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