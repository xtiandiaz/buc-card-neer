using System;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

public interface ICardView
{
    int Value { set; }
    Sprite FrontFace { set; }
    Sprite BackFace { set; }
    Vector3 Position { set; }
    Vector3 LocalPosition { set; }
    int SortingOrder { set; } 
    
    void OnPicked();
    IObservable<Unit> OnDropped();
    void Flip(CardFace toFace, bool animated);
    void MoveLocal(Vector3 toPosition);
    IObservable<Unit> MoveLocalAsObservable(Vector3 toPosition);
    void ToggleVisibility(bool toValue);
    void SetParent(Transform toTransform);
    void Fade(float toAlphaValue);
    IObservable<Unit> FadeAsObservable(float toAlphaValue);
    void Tint(Color withColor, float byFactor);
    void Fog(Color withColor, float byFactor);
    void Destroy();
}

public class CardView : MonoBehaviour, ICardView
{
    public class Factory : PlaceholderFactory<string, CardView>
    {
    }

    [SerializeField] protected CardLabel valueLabel;
    [SerializeField] private Transform contentWrapper;
    [SerializeField] private SortingGroup sortingGroup;
    [SerializeField] private CardFaceView frontFace;
    [SerializeField] private CardFaceView backFace;
    
    [Inject] private CardAnimationSettings animationSettings;
    private ICardAnimator animator;
    private ICardShader shader;
    private Vector3 lastLodgingPosition;

    public int Value
    {
        set
        {
            if (valueLabel != null)
                valueLabel.SetValue(value);
        }
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
        set
        {
            animator.Kill(CardAnimationType.Move);
            transform.position = value;
        }
    }
    
    public Vector3 LocalPosition
    {
        set
        {
            animator.Kill(CardAnimationType.Move);
            transform.localPosition = value;
        }
    }

    public int SortingOrder
    {
        set
        {
            sortingGroup.sortingOrder = value;

            var shouldToggleFaceContent = value >= -1;

            frontFace.ToggleContent(shouldToggleFaceContent);
            backFace.ToggleContent(shouldToggleFaceContent);
        }
    }

    private void Awake()
    {
        animator = GetComponent<ICardAnimator>() ?? gameObject.AddComponent<CardAnimator>();
        shader = GetComponent<ICardShader>();
        
        animator.Initialize(animationSettings, contentWrapper);
        shader.Initialize(animationSettings);
    }

    public void OnPicked()
    {
        animator.Kill(CardAnimationType.Move);
        animator.Lift();
    }

    public IObservable<Unit> OnDropped()
    {
        return animator.DropAsObservable();
    }

    public void ToggleVisibility(bool toValue)
    {
        gameObject.SetActive(toValue);
    }

    public void Fade(float toAlphaValue)
    {
        shader.Fade(toAlphaValue);
    }
    
    public IObservable<Unit> FadeAsObservable(float toAlphaValue)
    {
        return shader.FadeAsObservable(toAlphaValue);
    }

    public void Tint(Color withColor, float byFactor)
    {
        shader.Tint(withColor, byFactor);
    }
    
    public void Fog(Color withColor, float byFactor)
    {
        shader.Fog(withColor, byFactor);
    }

    public void Flip(CardFace toFace, bool animated)
    {
        animator.Flip(toFace, animated, () =>
        {
            frontFace.ToggleVisibility(toFace == CardFace.Front);
            backFace.ToggleVisibility(toFace == CardFace.Back);
        });
    }

    public void MoveLocal(Vector3 toPosition)
    {
        animator.MoveLocal(toPosition);
    }
    
    public IObservable<Unit> MoveLocalAsObservable(Vector3 toPosition)
    {
        return animator.MoveLocalAsObservable(toPosition, 0.5f);
    }
    
    public void SetParent(Transform toTransform)
    {
        transform.SetParent(toTransform, true);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}