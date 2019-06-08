using System;
using DG.Tweening;
using UniRx;
using UnityEngine;

public interface ICard : IDisposable
{
    CardType Type { get; }
    string Name { get; }
    int Value { get; set; }
    int OriginalValue { get; }
    int Index { get; }
    bool IsBoarded { get; set; }
    bool IsStored { get; set; }
    Vector3 LocalPosition { get; }
    DateTimeOffset BindingTimestamp { get; }

    IObservable<Unit> WhenPicked { get; }
    IObservable<Unit> WhenDropped { get; }
    IObservable<PlayerAttackType> WhenAttacked { get; }
    IObservable<Unit> WhenBounced { get; }
    IObservable<CardFace> WhenFlipped { get; }
    IObservable<Unit> WhenDestroyed { get; }

    void Construct(ICardView view);
    bool CanReflect(ICard other);
    bool CanMatch(ICard withOther);
    void Match(ICard withOther);
    bool CanClash(ICard other);
    IObservable<Unit> Clash(ICard other, Direction withDirection);
    bool CanBeStruck();
    void Strike(int withValue, PlayerAttackType andAttackType);
    void Bind(ICardBond withBond);
    void Pick();
    void Drag(Vector3 byDeltaPosition);
    void Drop();
    void Arrange(Vector3 atLocalPosition, float withRotationAngle, int andIndex);
    void Move(Vector3 toLocalPosition, CardMoveType withType);
    void Bounce();
    void Flip(CardFace toFace);
    void Rotate(float toAngle);
    IObservable<Unit> Fade(float toAlphaValue);
    void Fog(Color withColor, float byFactor);
    void Destroy();
}

public abstract class Card : ScriptableObject, ICard
{
    [SerializeField] protected IntReactiveProperty value = new IntReactiveProperty();
    
    private readonly Subject<Unit> picking = new Subject<Unit>();
    private readonly Subject<Unit> dropping = new Subject<Unit>();
    private readonly Subject<PlayerAttackType> striking = new Subject<PlayerAttackType>();
    private readonly Subject<Unit> bouncing = new Subject<Unit>();
    private readonly Subject<CardFace> flipping = new Subject<CardFace>();
    private readonly Subject<Unit> destruction = new Subject<Unit>();

    [SerializeField] private bool shouldDisplayValue = true;
    [SerializeField] private Sprite frontFace;
    [SerializeField] private Sprite backFace;

    private ICardView view;
    private ICardBond bond;
    private CardFace face;
    private Vector3 arrangedPosition;
    private float rotationAngle;

    public abstract CardType Type { get; }
    public string Name => name;
    
    public int Value
    {
        get => value.Value;
        set => this.value.Value = view.Value = Mathf.Max(value, 0);
    }

    public int OriginalValue { get; private set; }
    public int Index { get; private set; }
    public bool IsBoarded { get; set; }
    public bool IsStored { get; set; }
    public Vector3 LocalPosition { get; private set; }
    public DateTimeOffset BindingTimestamp { get; private set; }
    
    public IObservable<Unit> WhenPicked => picking;
    public IObservable<Unit> WhenDropped => dropping;
    public IObservable<PlayerAttackType> WhenAttacked => striking;
    public IObservable<Unit> WhenBounced => bouncing;
    public IObservable<CardFace> WhenFlipped => flipping.DistinctUntilChanged();
    public IObservable<Unit> WhenDestroyed => destruction;

    public void Construct(ICardView view)
    {
        OriginalValue = Value;
        
        view.FrontFace = frontFace;
        view.BackFace = backFace;
        view.Value = Value;
        view.ToggleValueVisibility(shouldDisplayValue);
        
        this.view = view;
    }

    public virtual bool CanReflect(ICard other)
    {
        return false;
    }

    public abstract bool CanMatch(ICard withOther);

    public abstract void Match(ICard withOther);

    public abstract bool CanClash(ICard other);

    public IObservable<Unit> Clash(ICard other, Direction withDirection)
    {
        return Observable.Create<Unit>(observer =>
        {
            Strike(1);

            var sequence = view.Tilt(withDirection, TimeSpan.FromSeconds(0.25))
                .OnComplete(observer.OnCompleted);

            return Disposable.Create(() => sequence.Kill());
        });
    }

    public abstract bool CanBeStruck();

    public void Strike(int withValue, PlayerAttackType andAttackType)
    {
        Strike(withValue);
        
        if (andAttackType == PlayerAttackType.Ranged) 
            view.Spin(2);

        striking.OnNext(andAttackType);
        
        if (Value <= 0)
            Destroy();
    }

    public void Bind(ICardBond withBond)
    {
        if (withBond == bond || withBond == null)
            return;
        
        bond?.Release(this);
        
        bond = withBond;
        BindingTimestamp = DateTimeOffset.Now;
        
        view.SetParent(withBond.TransformBond);
    }

    public void Pick()
    {       
        view.Pick();
        
        view.SortingOrder = 100;
        
        picking.OnNext(Unit.Default);
    }
    
    public void Drag(Vector3 byDeltaPosition)
    {
        LocalPosition += byDeltaPosition;
        
        view.LocalPosition = LocalPosition;
    }

    public void Drop()
    {
        LocalPosition = arrangedPosition;
        
        view.Drop(arrangedPosition)
            .OnComplete(() => Sort(Index));
        
        dropping.OnNext(Unit.Default);
    }

    public void Arrange(Vector3 atLocalPosition, float withRotationAngle, int andIndex)
    {
        arrangedPosition = atLocalPosition;
        Index = andIndex;
        
        Move(arrangedPosition, CardMoveType.Lodging);
        Rotate(withRotationAngle);
        Sort(andIndex);
    }

    public void Move(Vector3 toLocalPosition, CardMoveType withType)
    {
        LocalPosition = toLocalPosition;
        
        view.MoveLocal(toLocalPosition);
    }

    public void Rotate(float toAngle)
    {
        rotationAngle = toAngle;

        view.Rotate(Vector3.forward * rotationAngle);
    }

    public void Sort(int withIndex)
    {
        Index = withIndex;
        view.SortingOrder = -Index * 10;
    }
    
    public void Bounce()
    {
        bouncing.OnNext(Unit.Default);
    }

    public virtual void Flip(CardFace toFace)
    {
        face = toFace;

        view.Flip(face, true);
        
        flipping.OnNext(toFace);
    }
    
    public IObservable<Unit> Fade(float toAlphaValue)
    {
        return view.Fade(toAlphaValue);
    }

    public void Fog(Color withColor, float byFactor)
    {
        // TODO animate
        view.Fog(withColor, byFactor);
    }

    public virtual void Destroy()
    {
        bond?.Release(this);
        
        view.KillMove();
        view.FadeAwayAndDestroy();
        
        destruction.OnNext(Unit.Default);
        destruction.OnCompleted();
        
        Dispose();
    }

    public void Dispose()
    {
        value?.Dispose();
        picking?.Dispose();
        dropping?.Dispose();
        bouncing?.Dispose();
        flipping?.Dispose();
        destruction?.Dispose();
    }
    
    protected virtual void Strike(int withValue)
    {
        Value -= withValue;
    }
}