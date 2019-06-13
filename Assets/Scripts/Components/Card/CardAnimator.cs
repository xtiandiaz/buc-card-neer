using System;
using DG.Tweening;
using UniRx;
using UnityEngine;

public enum CardAnimationType
{
    Lift,
    Flip,
    Tilt,
    Move
}

public class CardAnimator : MonoBehaviour
{
    [SerializeField] private CardFaceView frontFace;
    [SerializeField] private CardFaceView backFace;
        
    private CardAnimationSettings settings;
    private Transform contentWrapper;
    private Tween moveTween, rotationTween;
    private Sequence flipSequence, tiltSequence, pickDropSequence;
    private CardFace currentFace;
    
    public void Initialize(CardAnimationSettings withSettings, Transform andContentWrapper)
    {
        settings = withSettings;
        contentWrapper = andContentWrapper;
    }

    public void Pick()
    {
        pickDropSequence?.Kill();
        moveTween?.Kill();

        pickDropSequence = DOTween.Sequence();
        
        pickDropSequence.Append(contentWrapper
            .DOMoveZ(0, settings.LiftDuration));

        pickDropSequence.Join(contentWrapper
            .DOScale(1.1f, settings.LiftDuration));
        
        pickDropSequence.SetEase(settings.OutEase);
    }

    public Tween Drop(Vector3 toLocalPosition)
    {
        pickDropSequence?.Kill();
        moveTween?.Kill();

        pickDropSequence.Append(contentWrapper
            .DOLocalMoveZ(0, settings.MoveDuration));
        
        pickDropSequence.Join(contentWrapper
            .DOScale(1f, settings.MoveDuration));

        pickDropSequence.SetEase(settings.OutEase);

        return MoveLocal(toLocalPosition);
    }

    public void Flip(CardFace toFace, bool whileAnimating)
    {
        Kill(CardAnimationType.Flip);

        currentFace = toFace;
        
        var destEulerAngles = GetRotationEulerAnglesDestination(currentFace);

        if (!whileAnimating)
        {
            transform.eulerAngles = destEulerAngles;
            ToggleFace(toFace);

            return;
        }

        var halfTweenDuration = settings.FlipDuration * 0.5f;

        flipSequence = DOTween.Sequence();
        
        flipSequence.Append(
            contentWrapper.DORotate(Vector3.up * 90f, halfTweenDuration)
                .OnComplete(() => ToggleFace(toFace))
                .SetEase(settings.InEase));

        flipSequence.Append(
            contentWrapper.DORotate(destEulerAngles, halfTweenDuration).SetEase(settings.OutEase));
    }

    public Sequence Tilt(Direction towardDirection, TimeSpan duringTime)
    {
        Kill(CardAnimationType.Flip);
        Kill(CardAnimationType.Tilt);
        
        tiltSequence = DOTween.Sequence();
        var originalRotation = GetRotationEulerAnglesDestination(currentFace);

        tiltSequence.Append(
            contentWrapper.DORotate(originalRotation + GetTiltingVector(towardDirection) * settings.TiltAngle, settings.TiltDuration)
                .SetEase(settings.OutEase));

        tiltSequence.Append(
            contentWrapper.DORotate(originalRotation, settings.TiltDuration)
                .SetDelay((float) duringTime.TotalSeconds)
                .SetEase(settings.OutEase));

        return tiltSequence;
    }

    public void Spin(int times)
    {
        Kill(CardAnimationType.Flip);
        Kill(CardAnimationType.Tilt);
        
        var originalRotation = GetRotationEulerAnglesDestination(currentFace);
        
        ToggleFaces(true);

        contentWrapper.DORotate(originalRotation - 360f * times * Vector3.up, settings.SpinDuration, RotateMode.FastBeyond360)
            .SetEase(settings.OutEase)
            .OnComplete(() => ToggleFace(currentFace));
    }

    public Tween MoveLocal(Vector3 toPosition)
    {
        Kill(CardAnimationType.Move);
        
        moveTween = transform.DOLocalMove(toPosition, settings.MoveDuration)
            .SetEase(settings.OutEase);

        return moveTween;
    }

    public void Rotate(Vector3 toEulerAngles)
    {
        rotationTween?.Kill();

        rotationTween = transform.DORotate(toEulerAngles, 0.25f)
            .SetEase(settings.OutEase);
    }
    
    public IObservable<Unit> MoveLocalAsObservable(Vector3 toPosition)
    {
        return Observable.Create<Unit>(observer =>
        {
            var tween = transform.DOLocalMove(toPosition, settings.MoveDuration)
                .SetEase(settings.OutEase)
                .OnComplete(() =>
                {
                    observer.OnNext(Unit.Default);
                    observer.OnCompleted();
                });

            return Disposable.Create(() => tween.Kill());
        });
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
                
                moveTween?.Kill();
                
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(animationType), animationType, null);
        }
    }

    private Tween Lift(float toDepth, float inSeconds)
    {
        return contentWrapper.DOMoveZ(0, settings.LiftDuration).SetEase(settings.OutEase);
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
    
    private void KillAll()
    {
        foreach (CardAnimationType animationType in Enum.GetValues(typeof(CardAnimationType)))
            Kill(animationType);
    }
}