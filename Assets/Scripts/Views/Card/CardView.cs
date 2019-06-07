using System;
using DG.Tweening;
using UniRx;
using UnityEngine;
using Zenject;

public interface ICardView
{
    int Value { set; }
    Sprite FrontFace { set; }
    Sprite BackFace { set; }
    Vector3 Position { set; }
    Vector3 LocalPosition { set; }
    int SortingOrder { set; }

    void Pick();
    Tween Drop(Vector3 toLocalPosition);
    void Flip(CardFace toFace, bool animated);
    Sequence Tilt(Direction towardDirection, TimeSpan duringTime);
    void Spin(int times);
    void MoveLocal(Vector3 toPosition);
    void Rotate(Vector3 toEulerAngles);
    void SetParent(Transform toTransform);
    void Fade(float toAlphaValue);
    IObservable<Unit> FadeAsObservable(float toAlphaValue);
    void Tint(Color withColor, float byFactor);
    void Fog(Color withColor, float byFactor);
    void KillMove();
    void ToggleValueVisibility(bool toValue);
    void Destroy();
}

public class CardView : MonoBehaviour, ICardView
{
    public class Factory : PlaceholderFactory<string, CardView>
    {
    }

    [SerializeField] protected CardLabel valueLabel;
    [SerializeField] protected CardFaceView frontFace;
    [SerializeField] protected CardFaceView backFace;
    [SerializeField] private Transform contentWrapper;

    [Inject] private CardAnimationSettings animationSettings;
    private CardAnimator animator;
    private ICardShader shader;
    private ISortingSet sortingSet;
    private Vector3 lastLodgingPosition;

    public virtual int Value
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
            sortingSet.SortingOrder = value;

            var shouldToggleFaceContent = value >= -2;

            frontFace.ToggleContent(shouldToggleFaceContent);
            backFace.ToggleContent(shouldToggleFaceContent);
        }
    }

    private void Awake()
    {
        animator = GetComponent<CardAnimator>();
        shader = GetComponent<ICardShader>();
        sortingSet = GetComponent<ISortingSet>();
        
        animator.Initialize(animationSettings, contentWrapper);
        shader.Initialize(animationSettings);
    }
    
    public void Pick()
    {
        animator.Pick();
    }

    public Tween Drop(Vector3 toLocalPosition)
    {
        return animator.Drop(toLocalPosition);
    }

    public void Flip(CardFace toFace, bool animated)
    {
        animator.Flip(toFace, animated);
    }

    public Sequence Tilt(Direction towardDirection, TimeSpan duringTime)
    {
        return animator.Tilt(towardDirection, duringTime);
    }

    public void Spin(int times)
    {
        animator.Spin(times);
    }

    public void MoveLocal(Vector3 toPosition)
    {
        animator.MoveLocal(toPosition);
    }

    public void Rotate(Vector3 toEulerAngles)
    {
        animator.Rotate(toEulerAngles);
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
    
    public void SetParent(Transform toTransform)
    {
        transform.SetParent(toTransform, true);
    }

    public void KillMove()
    {
        animator.Kill(CardAnimationType.Move);
    }

    public void ToggleValueVisibility(bool toValue)
    {
        valueLabel.ToggleVisibility(toValue);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}