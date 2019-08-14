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

    private ICardShader shader;
    private Tween rotation, depth;
    private Sequence flip;
    private CardFace currentFace = CardFace.Back;

    public void Initialize(ICardShader shader)
    {
        this.shader = shader;
    }
    
    public Sequence Arrange(CardArrangement withArrangement, CardArrangementMode andMode = CardArrangementMode.Normal)
    {
        shader.Fog(withArrangement.fogColor, withArrangement.fogIntensity);  //TODO animate fog
        
        var sequence = DOTween.Sequence();
        var duration = withArrangement.GetDuration(transform.localPosition, andMode);

        sequence.Append(transform.DOLocalMove(withArrangement.localPosition, duration)
            .SetEase(Ease.OutQuart));

        var eulerAngles = tweenWrapper.eulerAngles;
        eulerAngles.z = withArrangement.rotationZ;

        sequence.Join(Rotate(eulerAngles, duration));

        return sequence;
    }

    public Sequence Flip(CardFace toFace)
    {
        flip?.Kill();

        currentFace = toFace;
        
        var destEulerAngles = GetRotationEulerAnglesDestination(currentFace);
        var halfTweenDuration = 0.25f;

        flip = DOTween.Sequence();
        
        flip.Append(
            covers.DORotate(Vector3.up * 90f, halfTweenDuration)
                .SetEase(Ease.InQuart));

        flip.Join(TweenDepth(-2f, halfTweenDuration).SetEase(Ease.InQuart));

        flip.Append(
            covers.DORotate(destEulerAngles, halfTweenDuration).SetEase(Ease.OutQuart));
        
        flip.Join(TweenDepth(0, halfTweenDuration).SetEase(Ease.OutQuart));

        flip.OnStart(() => ToggleFaces(true));
        flip.OnComplete(() => ToggleFace(toFace));

        return flip;
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
        rotation?.Kill();

        rotation = tweenWrapper.DORotate(toEulerAngles, 0.25f)
            .SetEase(Ease.OutQuart);

        return rotation;
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
        depth?.Kill();

        depth = (shouldDoInLocalSpace
            ? tweenWrapper.DOLocalMoveZ(toValue, duringSeconds)
            : tweenWrapper.DOMoveZ(toValue, duringSeconds));

        return depth;
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
    
    private Tween Rotate(Vector3 toEulerAngles, float withDuration)
    {
        rotation?.Kill();

        rotation = tweenWrapper.DORotate(toEulerAngles, withDuration)
            .SetEase(Ease.OutQuart);

        return rotation;
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