using System;
using DG.Tweening;
using UniRx;
using UnityEngine;

public interface ICardView
{
    int Value { set; }
    int LockValue { set; }
    string Name { set; }
    CardFace Face { set; }
    ISuitModel Suit { set; }
    
    bool IsBoarded { set; }
    
    Sprite FrontCover { set; }
    Sprite BackCover { set; }
    Sprite FrontMotif { set; }
    Sprite BackMotif { set; }
    
    Vector3 Position { get; set; }
    Vector3 LocalPosition { get; }

    void Pick(Vector3 atPosition);
    void Drag(Vector3 toPosition);
    void Arrange(CardArrangement withArrangement);
    void ToggleValueVisibility(bool toValue);
    void ToggleLockVisibility(bool toValue);
    void Destroy();
    
    IObservable<Unit> Clash(Direction toward);
    IObservable<Unit> OnClashed();
    IObservable<Unit> OnShot();
    IObservable<Unit> Reveal();
    IObservable<Unit> Lodge(Transform inTransform, int withIndex, CardArrangement arrangement, CardArrangementMode andMode);
    IObservable<Unit> ArrangeAsObservable(CardArrangement withArrangement);
    IObservable<Unit> Fade(float toAlphaValue, TimeSpan withDuration);
}

public class CardView : MonoBehaviour, ICardView
{
    private readonly ReactiveProperty<bool> isBoarded = new ReactiveProperty<bool>(); 
        
    [SerializeField] protected CardCustomizer customizer = default;
    [SerializeField] private CardAnimator animator = default;
    [SerializeField] private CardSorter sorter = default;
    [SerializeField] private CardShader shader = default;

    [SerializeField] private Transform floatingWrapper = default;

    private bool isPicked;
    private float floatingT;
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

    public string Name
    {
        set => customizer.CardName = value;
    }

    public virtual ISuitModel Suit
    {
        set => customizer.Suit = value;
    }

    public bool IsBoarded
    {
        private get => isBoarded.Value;
        set => isBoarded.Value = value;
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

    private void Start()
    {
        Observable.EveryGameObjectUpdate()
            .TakeUntil(isBoarded.Where(value => value))
            .Do(_ => floatingT -= Time.deltaTime * 2f)
            .Where(_ => !isPicked)
            .Subscribe(_ =>
            {
                floatingWrapper.localPosition = 0.05f * Mathf.Sin(floatingT) * Vector3.up;
            })
            .AddTo(this);
    }

    public void Pick(Vector3 atPosition)
    {
        sorter.Order = 100;
        isPicked = true;

        arrangement?.Kill();

        Drag(atPosition);
    }

    public void Drag(Vector3 toPosition)
    {
        dragging?.Kill();
        dragging = transform.DOMove(toPosition, 0.1f)
            .SetEase(Ease.OutQuart);
    }

    public IObservable<Unit> Lodge(Transform inTransform, int withIndex, CardArrangement arrangement, CardArrangementMode andMode)
    {
        return Observable.Create<Unit>(observer =>
        {
            dragging?.Kill();
            
            transform.SetParent(inTransform, true);

            Sort(arrangement.index);

            if (!IsBoarded)
                floatingT = withIndex * Mathf.PI * 0.5f;
            
            isPicked = false;

            var sequence = animator.Arrange(arrangement, andMode)
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

        isPicked = false;
        
        arrangement = animator.Arrange(withArrangement)
            .OnComplete(() =>
            {
                Sort(withArrangement.index);
            });
    }
    
    public IObservable<Unit> ArrangeAsObservable(CardArrangement withArrangement)
    {
        return Observable.Create<Unit>(observer =>
        {
            dragging?.Kill();
            
            Sort(withArrangement.index);
            
            isPicked = false;
            
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

    /*private void TryFloating()
    {
        if (!IsBoarded)
            floating?.Play();
    }*/
}