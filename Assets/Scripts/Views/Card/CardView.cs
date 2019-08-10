using System;
using DG.Tweening;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;

public enum CardFace
{
    Front,
    Back
}

public interface ICardView
{
    int Value { set; }
    int LockValue { set; }
    CardFace Face { set; }
    ISuitModel Suit { set; }
    
    Sprite FrontCover { set; }
    Sprite BackCover { set; }
    Sprite FrontMotif { set; }
    Sprite BackMotif { set; }
    
    Vector3 Position { set; }
    Vector3 LocalPosition { get; }

    void Pick();
    void Drag(Vector3 byDeltaPosition);
    void Arrange(CardArrangement withArrangement);
    void ToggleValueVisibility(bool toValue);
    void ToggleLockVisibility(bool toValue);
    void Destroy();
    
    IObservable<Unit> Clash(Direction toward);
    IObservable<Unit> OnClashed();
    IObservable<Unit> OnShot();
    IObservable<Unit> Reveal();
    IObservable<Unit> Lodge(Transform inTransform, CardArrangement withArrangement, CardArrangementMode andMode);
    IObservable<Unit> ArrangeAsObservable(CardArrangement withArrangement);
    IObservable<Unit> Fade(float toAlphaValue, TimeSpan withDuration);
}

public class CardView : MonoBehaviour, ICardView
{
    [SerializeField] protected CardValue cardValue = default;
    [CanBeNull]
    [SerializeField] protected CardValue lockValue = default;
    
    [Space]
    [CanBeNull]
    [SerializeField] protected Suit suit = default;

    [Space]
    [SerializeField] protected CardCover frontCover = default;
    [SerializeField] protected CardCover backCover = default;
    
    [CanBeNull]
    [SerializeField] protected SpriteRenderer frontMotif = default;
    [CanBeNull] 
    [SerializeField] protected SpriteRenderer backMotif = default;
    
    [Space]
    [SerializeField] private Transform tweenWrapper = default;
    [SerializeField] private Transform covers = default;

    private CardAnimator animator;
    private ICardShader shader;
    private SortingGroup sortingGroup;
    private Vector3 lastLodgingPosition;
    private int sortingIndex;
    private CardFace face;
    
    
    private Tween picking, rotation;
    private Sequence arrangement;

    public virtual int Value
    {
        set
        {
            if (cardValue != null)
                cardValue.SetValue(value);
        }
    }

    public int LockValue
    {
        set
        {
            if (lockValue == null)
                return;
            
            lockValue.SetValue(value);
        }
    }

    public virtual ISuitModel Suit
    {
        set
        {
            if (suit != null && value != null) 
                suit.Customize(value);
        }
    }

    public CardFace Face
    {
        set
        {
            var eulerAngles = tweenWrapper.eulerAngles;
            eulerAngles.y = value == CardFace.Front ? 0 : 180f;
            
            covers.eulerAngles = eulerAngles;
            face = value;
            
            frontCover.ToggleVisibility(face == CardFace.Front);
            backCover.ToggleVisibility(face == CardFace.Back);
        }
    }
    
    public Sprite FrontCover
    {
        set => frontCover.Cover = value;
    }

    public Sprite BackCover
    {
        set => backCover.Cover = value;
    }
    
    public Sprite FrontMotif
    {
        set
        {
            if (frontMotif != null) 
                frontMotif.sprite = value;
        }
    }

    public Sprite BackMotif
    {
        set
        {
            if (backMotif != null) 
                backMotif.sprite = value;
        }
    }

    public Vector3 Position
    {
        set => transform.position = value;
    }
    
    public Vector3 LocalPosition
    {
        get => transform.localPosition;
        private set => transform.localPosition = value;
    }

    private int SortingOrder
    {
        get => sortingGroup.sortingOrder;
        set
        {
            sortingGroup.sortingOrder = value;

            var shouldToggleFaceContent = value >= -2;

            frontCover.ToggleContent(shouldToggleFaceContent);
            backCover.ToggleContent(shouldToggleFaceContent);
        }
    }

    private void Awake()
    {
        animator = GetComponent<CardAnimator>();
        sortingGroup = GetComponent<SortingGroup>();
        shader = GetComponent<ICardShader>();
        
        animator.Initialize(shader);
    }
    
    public void Pick()
    {
        SortingOrder = 100;

        arrangement?.Kill();
        picking?.Kill();
        
        picking = transform.DOMoveZ(-0.5f, 0.2f)
            .SetEase(Ease.OutQuart);
    }

    public void Drag(Vector3 byDeltaPosition)
    {
        LocalPosition += byDeltaPosition;
    }

    public IObservable<Unit> Lodge(Transform inTransform, CardArrangement withArrangement, CardArrangementMode andMode)
    {
        return Observable.Create<Unit>(observer =>
        {
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
        arrangement?.Kill();
        arrangement = animator.Arrange(withArrangement)
            .OnComplete(() => Sort(withArrangement.index));
    }
    
    public IObservable<Unit> ArrangeAsObservable(CardArrangement withArrangement)
    {
        return Observable.Create<Unit>(observer =>
        {
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
                    
                    backCover.ToggleVisibility(false);
                    
                    observer.OnNext(Unit.Default);
                    observer.OnCompleted();
                });
            
            return Disposable.Create(() => sequence.Kill());
        }); 
    }

    public IObservable<Unit> Clash(Direction toward)
    {
        return animator.Clash(toward)
            .DoOnSubscribe(() => SortingOrder += 1)
            .DoOnCompleted(() => SortingOrder -= 1);
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
        cardValue.ToggleVisibility(toValue);
    }
    
    public void ToggleLockVisibility(bool toValue)
    {
        if (lockValue != null) 
            lockValue.ToggleVisibility(toValue);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
    
    private void Sort(int withRawIndex)
    {
        SortingOrder = -withRawIndex;
    }
}