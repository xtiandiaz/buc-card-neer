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

    void Lift();
    void Drop();
    void Flip(CardFace toFace, bool animated);
    void Tilt(Direction towardDirection, TimeSpan duringTime);
    void Spin(int times);
    Tween MoveLocal(Vector3 toPosition);
    IObservable<Unit> MoveLocalAsObservable(Vector3 toPosition);
    void SetParent(Transform toTransform);
    void Fade(float toAlphaValue);
    IObservable<Unit> FadeAsObservable(float toAlphaValue);
    void Tint(Color withColor, float byFactor);
    void Fog(Color withColor, float byFactor);
    void Halt();
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
    private ICardAnimator animator;
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
        set => sortingSet.SortingOrder = value;
    }

    private void Awake()
    {
        animator = GetComponent<ICardAnimator>();
        shader = GetComponent<ICardShader>();
        sortingSet = GetComponent<ISortingSet>();
        
        animator.Initialize(animationSettings, contentWrapper);
        shader.Initialize(animationSettings);
    }
    
    public void Lift()
    {
        animator.Kill(CardAnimationType.Move);
        animator.Lift();
    }

    public void Drop()
    {
        animator.Drop();
    }

    public void Flip(CardFace toFace, bool animated)
    {
        animator.Flip(toFace, animated);
    }

    public void Tilt(Direction towardDirection, TimeSpan duringTime)
    {
        animator.Tilt(towardDirection, duringTime);
    }

    public void Spin(int times)
    {
        animator.Spin(times);
    }

    public Tween MoveLocal(Vector3 toPosition)
    {
        return animator.MoveLocal(toPosition);
    }

    public IObservable<Unit> MoveLocalAsObservable(Vector3 toPosition)
    {
        return animator.MoveLocalAsObservable(toPosition);
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

    public void Halt()
    {
        animator.Kill(CardAnimationType.Move);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}