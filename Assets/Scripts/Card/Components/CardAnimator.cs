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
    [SerializeField] private CardShader shader = default;
    
    [Space]
    [SerializeField] private Transform tweenWrapper = default;
    [SerializeField] private Transform covers = default;
    
    private CardFace currentFace = CardFace.Back;

    public Sequence Fling(Vector3 toPosition, Ease withEase, float andDuration)
    {
        var sequence = DOTween.Sequence();
        
        sequence.Append(transform.DOMove(toPosition, andDuration));
        sequence.Append(transform.DORotate(Vector3.up * 720f, andDuration, RotateMode.FastBeyond360));

        sequence.SetEase(withEase);
        sequence.OnStart(() => ToggleFaces(true));
        sequence.OnComplete(() => ToggleFace(currentFace));

        return sequence;
    }

    public Sequence Arrange(ArrangementInfo withInfo)
    {
        var duration = withInfo.GetDuration(transform.localPosition);
        var sequence = DOTween.Sequence();

        sequence.Append(transform.DOLocalMove(withInfo.LocalPosition, duration)
            .SetEase(withInfo.Ease));
        
        if (withInfo.FogColor.HasValue)
        {
            sequence.Join(
                shader.Fog(withInfo.FogColor.Value, withInfo.FogIntensity, withInfo.Ease, duration));
        }

        var eulerAngles = tweenWrapper.eulerAngles;
        eulerAngles.z = withInfo.RotationZ;

        sequence.Join(tweenWrapper.DORotate(eulerAngles, duration)
            .SetEase(withInfo.Ease));

        return sequence;
    }

    public Sequence Flip(CardFace toFace)
    {
        currentFace = toFace;
        
        var destEulerAngles = GetRotationEulerAnglesDestination(currentFace);
        var halfTweenDuration = 0.25f;
        var sequence = DOTween.Sequence();

        sequence.Append(covers.DORotate(Vector3.up * 90f, halfTweenDuration)
            .SetEase(Ease.InQuart));

        sequence.Join(covers.DOLocalMoveZ(-2f, halfTweenDuration).SetEase(Ease.InQuart));
        sequence.Append(covers.DORotate(destEulerAngles, halfTweenDuration).SetEase(Ease.OutQuart));
        sequence.Join(covers.DOLocalMoveZ(0, halfTweenDuration).SetEase(Ease.OutQuart));

        sequence.OnStart(() => ToggleFaces(true));
        sequence.OnComplete(() => ToggleFace(toFace));

        return sequence;
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

    [UsedImplicitly]
    public void OnTimelineAnimationFinished(CardTimelineAnimationKey withKey)
    {
        timelineAnimationCompletion.OnNext(withKey);
        
        if (withKey == CardTimelineAnimationKey.RangeShot)
            ToggleFace(currentFace);
    }

    public void Bounce(Vector3 withVector, float andDuration)
    {
        tweenWrapper.DOPunchPosition(withVector, andDuration, 5);
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
            })
            .TakeUntilDestroy(this);
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