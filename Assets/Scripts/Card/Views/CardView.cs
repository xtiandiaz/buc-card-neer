using System;
using DG.Tweening;
using UniRx;
using UnityEngine;

public class CardView : MonoBehaviour, ICardView
{
    [SerializeField] protected CardCustomizer customizer = default;
    [SerializeField] private CardAnimator animator = default;
    [SerializeField] private CardSorter sorter = default;
    [SerializeField] private CardShader shader = default;

    private Tween dragging;
    private Sequence arrangement;

    public virtual int Value
    {
        set => customizer.Value = value;
    }

    public int LockValue
    {
        set => customizer.LockValue = value;
    }

    public virtual ISuitModel Suit
    {
        set => customizer.Suit = value;
    }

    public Sprite FrontCover
    {
        set => customizer.FrontCover = value;
    }

    public Sprite BackCover
    {
        set => customizer.BackCover = value;
    }
    
    public Sprite FrontMotif
    {
        set => customizer.FrontMotif = value;
    }

    public Sprite BackMotif
    {
        set => customizer.BackMotif = value;
    }
    
    public CardFace Face
    {
        set => customizer.Face = value;
    }

    public Vector3 Position
    {
        get => transform.position;
        set => transform.position = value;
    }
    
    public Vector3 LocalPosition => transform.localPosition;

    public void Pick(Vector3 atPosition)
    {
        sorter.Order = 100;

        arrangement?.Kill();

        Drag(atPosition);
    }

    public void Drag(Vector3 toPosition)
    {
        dragging?.Kill();
        dragging = transform.DOMove(toPosition, 0.25f)
            .SetEase(Ease.OutQuart);
    }

    public IObservable<Unit> Lodge(Transform inTransform, CardArrangement withArrangement, CardArrangementMode andMode)
    {
        return Observable.Create<Unit>(observer =>
        {
            dragging?.Kill();
            
            transform.SetParent(inTransform, true);

            Sort(withArrangement.index);

            var sequence = animator.Arrange(withArrangement, andMode)
                .OnComplete(() => 
                {
                    observer.OnNext(Unit.Default);
                    observer.OnCompleted();
                });
            
            return Disposable.Create(() => sequence.Kill());
        });
    }

    public void Arrange(CardArrangement withArrangement)
    {
        dragging?.Kill();
        arrangement?.Kill();
        
        arrangement = animator.Arrange(withArrangement)
            .OnComplete(() => Sort(withArrangement.index));
    }
    
    public IObservable<Unit> ArrangeAsObservable(CardArrangement withArrangement)
    {
        return Observable.Create<Unit>(observer =>
        {
            dragging?.Kill();
            
            Sort(withArrangement.index);
            
            var sequence = animator.Arrange(withArrangement);

            sequence.OnComplete(() =>
            {
                observer.OnNext(Unit.Default);
                observer.OnCompleted();
            });

            return Disposable.Create(() => arrangement.Kill());
        });
    }

    public IObservable<Unit> Reveal()
    {
        return Observable.Create<Unit>(observer =>
        {
            var sequence = animator.Flip(CardFace.Front)
                .OnComplete(() =>
                {
                    Face = CardFace.Front;

                    observer.OnNext(Unit.Default);
                    observer.OnCompleted();
                });
            
            return Disposable.Create(() => sequence.Kill());
        }); 
    }

    public IObservable<Unit> Clash(Direction toward)
    {
        return animator.Clash(toward)
            .DoOnSubscribe(() => sorter.Order += 1)
            .DoOnCompleted(() => sorter.Order -= 1);
    }

    public IObservable<Unit> OnClashed()
    {
        return animator.OnClashed();
    }
    
    public IObservable<Unit> OnShot()
    {
        return animator.OnShot();
    } 

    public IObservable<Unit> Fade(float toAlphaValue, TimeSpan withDuration)
    {
        return shader.Fade(toAlphaValue, withDuration);
    }

    public void ToggleValueVisibility(bool toValue)
    {
        customizer.ToggleValueVisibility(toValue);
    }
    
    public void ToggleLockVisibility(bool toValue)
    {
        customizer.ToggleLockVisibility(toValue);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
    
    private void Sort(int withRawIndex)
    {
        sorter.Order = -withRawIndex;
    }
}