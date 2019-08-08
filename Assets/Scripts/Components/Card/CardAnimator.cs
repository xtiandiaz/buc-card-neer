using System;
using DG.Tweening;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;
using Zenject;

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
    private CardFace currentFace = CardFace.Back;

    public Sequence Flip(CardFace toFace)
    {
        flipSequence?.Kill();

        currentFace = toFace;
        
        var destEulerAngles = GetRotationEulerAnglesDestination(currentFace);
        var halfTweenDuration = 0.25f;

        flipSequence = DOTween.Sequence();
        
        flipSequence.Append(
            covers.DORotate(Vector3.up * 90f, halfTweenDuration)
                .SetEase(Ease.InQuart));

        flipSequence.Join(TweenDepth(-2f, halfTweenDuration).SetEase(Ease.InQuart));

        flipSequence.Append(
            covers.DORotate(destEulerAngles, halfTweenDuration).SetEase(Ease.OutQuart));
        
        flipSequence.Join(TweenDepth(0, halfTweenDuration).SetEase(Ease.OutQuart));

        flipSequence.OnStart(() => ToggleFaces(true));
        flipSequence.OnComplete(() => ToggleFace(toFace));

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
        
        if (withKey == CardTimelineAnimationKey.RangeShot)
            ToggleFace(currentFace);
    }
    
    private Tween TweenDepth(float toValue, float duringSeconds, bool shouldDoInLocalSpace = true)
    {
        depthTween?.Kill();

        depthTween = (shouldDoInLocalSpace
            ? tweenWrapper.DOLocalMoveZ(toValue, duringSeconds)
            : tweenWrapper.DOMoveZ(toValue, duringSeconds));

        return depthTween;
    }

    private IObservable<Unit> PlayTimelineAnimation(string withName, CardTimelineAnimationKey andKey)
    {
        return Observable.Create<Unit>(observer =>
        {
            if (andKey == CardTimelineAnimationKey.RangeShot)
                ToggleFaces(true);
            
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

    private void ToggleFaces(bool toValue)
    {
        frontFace.ToggleVisibility(toValue);
        backFace.ToggleVisibility(toValue);
    }

    private void OnDestroy()
    {
        timelineAnimationCompletion.Dispose();
    }
}