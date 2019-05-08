using System;
using System.Runtime.InteropServices.WindowsRuntime;
using DG.Tweening;
using UniRx;
using UnityEngine;

public enum CardAnimationType
{
    Lift,
    Flip,
    Move
}

public interface ICardAnimator
{
    void Initialize(CardAnimationSettings withSettings, Transform andContentWrapper);
    void Lift();
    void Drop();
    IObservable<Unit> DropAsObservable();
    void Flip(CardFace toFace, bool whileAnimating, Action andDoAmidFlip = null);
    Tween Move(Vector3 toPosition);
    IObservable<Unit> MoveAsObservable(Vector3 toPosition, float duringSeconds);
    void Kill(CardAnimationType animationType);
}

public class CardAnimator : MonoBehaviour, ICardAnimator
{
    private CardAnimationSettings settings;
    private Transform contentWrapper;
    private Tween moveTween, liftTween;
    private Sequence flipSequence;

    public void Initialize(CardAnimationSettings withSettings, Transform andContentWrapper)
    {
        settings = withSettings;
        contentWrapper = andContentWrapper;
    }

    public void Lift()
    {
        Kill(CardAnimationType.Lift);
        liftTween = Lift(-settings.LiftDepth, settings.LiftDuration);
    }

    public void Drop()
    {
        Kill(CardAnimationType.Lift);
        liftTween = Lift(0, settings.ReturnToLocationWerePickedDuration);
    }
    
    public IObservable<Unit> DropAsObservable()
    {
        return Observable.Create<Unit>(observer =>
        {
            var tween = Lift(0, settings.LiftDuration);

            return Disposable.Create(() => tween.Kill());
        });
    }
    
    public void Flip(CardFace toFace, bool whileAnimating, Action andDoAmidFlip = null)
    {
        Kill(CardAnimationType.Flip);
        Kill(CardAnimationType.Lift);

        var destEulerAngles = Vector3.up * (toFace == CardFace.Back ? 180f : 0);

        if (!whileAnimating)
        {
            transform.eulerAngles = destEulerAngles;
            andDoAmidFlip?.Invoke();

            return;
        }

        var halfTweenDuration = settings.FlipDuration * 0.5f;

        flipSequence = DOTween.Sequence();
        
        flipSequence.Append(
            contentWrapper.DORotate(Vector3.up * 90f, halfTweenDuration)
                .OnComplete(() => andDoAmidFlip?.Invoke())
                .SetEase(settings.InEase));

        flipSequence.Join(Lift(-settings.LiftDepth, halfTweenDuration).SetEase(settings.InEase));

        flipSequence.Append(
            contentWrapper.DORotate(destEulerAngles, halfTweenDuration).SetEase(settings.OutEase));

        flipSequence.Join(Lift(0, halfTweenDuration).SetEase(settings.OutEase));
    }

    public void Kill(CardAnimationType animationType)
    {
        switch (animationType)
        {
            case CardAnimationType.Lift:
                
                liftTween?.Kill();
                
                break;
            case CardAnimationType.Flip:
                
                flipSequence?.Kill();
                
                break;
            case CardAnimationType.Move:
                
                moveTween?.Kill();
                
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(animationType), animationType, null);
        }
    }
    
    public Tween Move(Vector3 toPosition)
    {
        Kill(CardAnimationType.Move);
        
        moveTween = transform.DOMove(toPosition, settings.MoveDuration)
            .SetEase(settings.OutEase);

        return moveTween;
    }
    
    public IObservable<Unit> MoveAsObservable(Vector3 toPosition, float duringSeconds)
    {
        return Observable.Create<Unit>(observer =>
        {
            var tween = transform.DOMove(toPosition, duringSeconds)
                .SetEase(settings.OutEase)
                .OnComplete(() =>
                {
                    observer.OnNext(Unit.Default);
                    observer.OnCompleted();
                });

            return Disposable.Create(() => tween.Kill());
        });
    }
    
    private Tween Lift(float toDepth, float inSeconds)
    {
        return contentWrapper.DOLocalMoveZ(toDepth, settings.LiftDuration).SetEase(settings.OutEase);
    }
}