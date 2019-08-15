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

    public Sequence Float(float withExtent, float andDurationPerLoop)
    {
        var sequence = DOTween.Sequence();
        
        sequence.Append(tweenWrapper.DOLocalMoveY(withExtent, andDurationPerLoop * 0.25f)
            .SetEase(Ease.OutSine));
        sequence.Append(tweenWrapper.DOLocalMoveY(-withExtent, andDurationPerLoop * 0.5f)
            .SetEase(Ease.InOutSine));
        sequence.Append(tweenWrapper.DOLocalMoveY(0, andDurationPerLoop * 0.25f)
            .SetEase(Ease.InSine));
        
        sequence.SetLoops(-1);

        return sequence;
    }

    public Sequence Arrange(CardArrangement withArrangement, CardArrangementMode andMode = CardArrangementMode.Normal)
    {
        shader.Fog(withArrangement.fogColor, withArrangement.fogIntensity);  //TODO animate fog
        
        var duration = withArrangement.GetDuration(transform.localPosition, andMode);
        var sequence = DOTween.Sequence();

        sequence.Append(transform.DOLocalMove(withArrangement.localPosition, duration)
            .SetEase(Ease.OutQuart));

        var eulerAngles = tweenWrapper.eulerAngles;
        eulerAngles.z = withArrangement.rotationZ;

        sequence.Join(tweenWrapper.DORotate(eulerAngles, duration)
            .SetEase(Ease.OutQuart));

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