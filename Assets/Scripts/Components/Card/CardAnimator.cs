using System;
using DG.Tweening;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;

public enum CardTimelineAnimationKey
{
    Clash,
    ClashImpact,
    RangeShot
}

public class CardAnimator : MonoBehaviour
{
    private readonly Subject<CardTimelineAnimationKey> timelineAnimationCompletion = new Subject<CardTimelineAnimationKey>();

    [SerializeField] private CardCover frontFace = default;
    [SerializeField] private CardCover backFace = default;
    
    [Space]
    [SerializeField] private Animator animator = default;
    
    [Space]
    [SerializeField] private Transform tweenWrapper = default;
    [SerializeField] private Transform covers = default;

    private Tween rotationTween, depthTween;
    private Sequence flipSequence;
    private CardFace currentFace;

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
        var duration = 0.4f;

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
        flipSequence?.Kill();

        currentFace = toFace;
        
        var destEulerAngles = GetRotationEulerAnglesDestination(currentFace);
        var halfTweenDuration = 0.2f;

        flipSequence = DOTween.Sequence();
        
        flipSequence.Append(
            covers.DORotate(Vector3.up * 90f, halfTweenDuration)
                .OnComplete(() => ToggleFace(toFace))
                .SetEase(Ease.InQuart));

        flipSequence.Join(TweenDepth(-1.5f, halfTweenDuration).SetEase(Ease.InQuart));

        flipSequence.Append(
            covers.DORotate(destEulerAngles, halfTweenDuration).SetEase(Ease.OutQuart));
        
        flipSequence.Join(TweenDepth(0, halfTweenDuration).SetEase(Ease.OutQuart));

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
            .SetEase(Ease.OutQuart);

        return rotationTween;
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

    private Vector3 GetRotationEulerAnglesDestination(CardFace forFace)
    {
        return Vector3.up * (forFace == CardFace.Back ? 180f : 0);
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