using System;
using DG.Tweening;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;
using Zenject;

public enum CardAnimationType
{
    Lift,
    Flip,
    Tilt,
    Move
}

public enum CardTimelineAnimationKey
{
    Clash,
    ClashImpact,
    RangeShot
}

public class CardAnimator : MonoBehaviour
{
    private readonly Subject<CardTimelineAnimationKey> timelineAnimationCompletion = new Subject<CardTimelineAnimationKey>();

    [SerializeField] private CardCover frontFace;
    [SerializeField] private CardCover backFace;
    
    [Space]
    [SerializeField] private Animator animator;
    
    [Space]
    [SerializeField] private Transform tweenWrapper;
    [SerializeField] private Transform covers;

    private CardAnimationSettings settings;
    private Tween rotationTween, depthTween;
    private Sequence moveSequence;
    private Sequence flipSequence, tiltSequence, pickDropSequence;
    private CardFace currentFace;

    private Viewport viewport;

    public void Initialize(CardAnimationSettings withSettings, Viewport viewport)
    {
        settings = withSettings;

        this.viewport = viewport;
    }

    public Tween TweenDepth(float toValue, float duringSeconds, bool shouldDoInLocalSpace = true)
    {
        depthTween?.Kill();

        depthTween = (shouldDoInLocalSpace
            ? tweenWrapper.DOLocalMoveZ(toValue, duringSeconds)
            : tweenWrapper.DOMoveZ(toValue, duringSeconds));

        return depthTween;
    }
    
    public Sequence Arrange(Vector3 atLocalPosition, float withRotationAngle)
    {
        var sequence = DOTween.Sequence();
        var duration = settings.MoveDuration;

        sequence.Append(transform.DOLocalMove(atLocalPosition, duration)
            .SetEase(Ease.OutQuart));
        
        sequence.Join(TweenDepth(0, duration).SetEase(Ease.OutQuart));

        var eulerAngles = tweenWrapper.eulerAngles;
        eulerAngles.z = withRotationAngle;
        
        sequence.Join(Rotate(eulerAngles));

        return sequence;
    }

    public Sequence Flip(CardFace toFace)
    {
        Kill(CardAnimationType.Flip);

        currentFace = toFace;
        
        var destEulerAngles = GetRotationEulerAnglesDestination(currentFace);
        var halfTweenDuration = 0.2f;

        flipSequence = DOTween.Sequence();
        
        flipSequence.Append(
            covers.DORotate(Vector3.up * 90f, halfTweenDuration)
                .OnComplete(() => ToggleFace(toFace))
                .SetEase(settings.InEase));

        flipSequence.Join(TweenDepth(-1.5f, halfTweenDuration).SetEase(settings.InEase));

        flipSequence.Append(
            covers.DORotate(destEulerAngles, halfTweenDuration).SetEase(settings.OutEase));
        
        flipSequence.Join(TweenDepth(0, halfTweenDuration).SetEase(settings.OutEase));

        return flipSequence;
    }

    public IObservable<Unit> Clash(Direction toward)
    {
        return PlayTimelineAnimation($"Clash{toward}", CardTimelineAnimationKey.Clash);
    }
    
    public IObservable<Unit> OnClashed()
    {
        return PlayTimelineAnimation("ClashImpact", CardTimelineAnimationKey.ClashImpact);
    }
    
    public IObservable<Unit> OnShot()
    {
        return PlayTimelineAnimation("RangeShot", CardTimelineAnimationKey.RangeShot);
    }

    public Tween Rotate(Vector3 toEulerAngles)
    {
        rotationTween?.Kill();

        rotationTween = tweenWrapper.DORotate(toEulerAngles, 0.25f)
            .SetEase(settings.OutEase);

        return rotationTween;
    }

    public void Kill(CardAnimationType animationType)
    {
        switch (animationType)
        {
            case CardAnimationType.Lift:
                
                pickDropSequence?.Kill();
                
                break;
            case CardAnimationType.Flip:
                
                flipSequence?.Kill();
                
                break;
            case CardAnimationType.Tilt:
                
                tiltSequence?.Kill();
                
                break;
            case CardAnimationType.Move:
                
                moveSequence?.Kill();
                
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(animationType), animationType, null);
        }
    }
    
    [UsedImplicitly]
    public void OnTimelineAnimationFinished(CardTimelineAnimationKey withKey)
    {
        timelineAnimationCompletion.OnNext(withKey);
    }

    private IObservable<Unit> PlayTimelineAnimation(string withName, CardTimelineAnimationKey andKey)
    {
        return Observable.Create<Unit>(observer =>
        {
            animator.Play(withName);
            
            return timelineAnimationCompletion
                .First(anim => anim == andKey)
                .AsUnitObservable()
                .Subscribe(observer);
        });
    }

    private Tween Lift(float toDepth, float inSeconds)
    {
        return tweenWrapper.DOMoveZ(0, settings.LiftDuration).SetEase(settings.OutEase);
    }

    private Vector3 GetRotationEulerAnglesDestination(CardFace forFace)
    {
        return Vector3.up * (forFace == CardFace.Back ? 180f : 0);
    }

    private Vector3 GetTiltingVector(Direction fromDirection)
    {
        switch (fromDirection)
        {
            case Direction.Up:
                return Vector3.right;
            case Direction.Down:
                return Vector3.left;
            case Direction.Right:
                return Vector3.down;
            case Direction.Left:
                return Vector3.up;
            default:
                return Vector3.zero;       
        }
    }
    
    private void ToggleFace(CardFace toValue)
    {
        frontFace.ToggleVisibility(toValue == CardFace.Front);
        backFace.ToggleVisibility(toValue == CardFace.Back);
    }

    private void ToggleFaces(bool on)
    {
        frontFace.ToggleVisibility(on);
        backFace.ToggleVisibility(on);
    }

    private void OnDestroy()
    {
        timelineAnimationCompletion.Dispose();
    }
}