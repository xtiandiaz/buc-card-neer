using System;
using UniRx;
using UnityEngine;

public interface ICard
{
    int Value { get; set; }
    int OriginalValue { get; }
    int Index { get; }
    string Name { get; }
    CardType Type { get; }
    bool IsBoarded { get; set; }
    bool IsStored { get; set; }
    bool ShouldDisplayValue { get; }
    ICardBond Bond { get; }
    DateTimeOffset BindingTimestamp { get; }
    Sprite FrontFace { get; }
    Sprite BackFace { get; }
    Vector3 LocalPosition { get; }
    Vector3 ArrangedPosition { get; }
    float RotationAngle { get; }
    
    IObservable<int> ValueAsObservable { get; }
    IObservable<Unit> WhenArranged { get; }
    IObservable<Direction> WhenClashed { get; }
    IObservable<PlayerAttackType> WhenStruck { get; }
    IObservable<Transform> WhenBound { get; }
    IObservable<Unit> WhenPicked { get; }
    IObservable<Unit> WhenDragged { get; }
    IObservable<Unit> WhenDropped { get; }
    IObservable<CardMoveType> WhenMoved { get; }
    IObservable<Unit> WhenBounced { get; }
    IObservable<CardFace> WhenFlipped { get; }
    IObservable<Unit> WhenRotated { get; }
    IObservable<float> WhenFaded { get; }
    IObservable<(Color, float)> WhenTinted { get; }
    IObservable<(Color, float)> WhenFogged { get; }
    IObservable<Unit> WhenDestroyed { get; }

    bool CanReflect(ICard other);
    bool CanMatch(ICard withOther);
    void Match(ICard withOther);
    bool CanClash(ICard other);
    void Clash(ICard other, Direction withDirection);
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
    void Fade(float toAlphaValue);
    void Tint(Color withColor, float byFactor);
    void Fog(Color withColor, float byFactor);
    void Destroy();
}

public abstract class Card : ScriptableObject, ICard
{
    [SerializeField] protected IntReactiveProperty value = new IntReactiveProperty();
    
    private readonly Subject<Direction> clashing = new Subject<Direction>();
    private readonly Subject<PlayerAttackType> striking = new Subject<PlayerAttackType>();
    private readonly Subject<Transform> binding = new Subject<Transform>();
    private readonly Subject<Unit> arrangement = new Subject<Unit>();
    private readonly Subject<Unit> picking = new Subject<Unit>();
    private readonly Subject<Unit> dragging = new Subject<Unit>();
    private readonly Subject<Unit> dropping = new Subject<Unit>();
    private readonly Subject<CardMoveType> movement = new Subject<CardMoveType>();
    private readonly Subject<Unit> bouncing = new Subject<Unit>();
    private readonly Subject<CardFace> flipping = new Subject<CardFace>();
    private readonly Subject<Unit> rotation = new Subject<Unit>();
    private readonly Subject<float> fading = new Subject<float>();
    private readonly Subject<(Color, float)> tinting = new Subject<(Color, float)>();
    private readonly Subject<(Color, float)> fogging = new Subject<(Color, float)>();
    private readonly Subject<Unit> destruction = new Subject<Unit>();

    [SerializeField] private bool shouldDisplayValue = true;
    [SerializeField] private Sprite frontFace;
    [SerializeField] private Sprite backFace;
    
    private CardFace face;
    private float arrangedDepth;

    public int Value
    {
        get => value.Value;
        set => this.value.Value = Mathf.Max(value, 0);
    }

    public int OriginalValue { get; private set; }
    public int Index { get; private set; }
    
    public abstract CardType Type { get; }
    public string Name => name;
    public bool IsBoarded { get; set; }
    public bool IsStored { get; set; }
    public bool ShouldDisplayValue => shouldDisplayValue;
    public ICardBond Bond { get; private set; }
    public DateTimeOffset BindingTimestamp { get; private set; }
    public Sprite FrontFace => frontFace;
    public Sprite BackFace => backFace;
    public Vector3 LocalPosition { get; private set; }
    public Vector3 ArrangedPosition { get; private set; }
    public float RotationAngle { get; private set; }

    public IObservable<int> ValueAsObservable => value;
    public IObservable<Unit> WhenArranged => arrangement;
    public IObservable<Direction> WhenClashed => clashing;
    public IObservable<PlayerAttackType> WhenStruck => striking;
    public IObservable<Transform> WhenBound => binding;
    public IObservable<Unit> WhenPicked => picking;
    public IObservable<Unit> WhenDragged => dragging;
    public IObservable<Unit> WhenDropped => dropping;
    public IObservable<CardMoveType> WhenMoved => movement;
    public IObservable<Unit> WhenBounced => bouncing;
    public IObservable<CardFace> WhenFlipped => flipping.DistinctUntilChanged();
    public IObservable<Unit> WhenRotated => rotation;
    public IObservable<float> WhenFaded => fading;
    public IObservable<(Color, float)> WhenTinted => tinting;
    public IObservable<(Color, float)> WhenFogged => fogging;
    public IObservable<Unit> WhenDestroyed => destruction;

    protected virtual void Awake()
    {
        OriginalValue = Value;
    }

    public virtual bool CanReflect(ICard other)
    {
        return false;
    }

    public abstract bool CanMatch(ICard withOther);

    public abstract void Match(ICard withOther);

    public abstract bool CanClash(ICard other);

    public void Clash(ICard other, Direction withDirection)
    {
        /* Note that the other Card isn't Struck upon Clashing; we simply affect its Value.
         * This is to prevent side-effects associated with deliberate attacks by the player.
         */
        other.Value--;

        clashing.OnNext(withDirection);
    }

    public abstract bool CanBeStruck();

    public void Strike(int withValue, PlayerAttackType andAttackType)
    {
        Strike(withValue);
        
        striking.OnNext(andAttackType);
    }

    public void Bind(ICardBond withBond)
    {
        if (withBond == Bond || withBond == null)
            return;
        
        Bond?.Release(this);
        
        Bond = withBond;
        BindingTimestamp = DateTimeOffset.Now;

        binding.OnNext(withBond.TransformBond);
    }

    public void Pick()
    {       
        picking.OnNext(Unit.Default);
    }
    
    public void Drag(Vector3 byDeltaPosition)
    {        
        LocalPosition += byDeltaPosition;
        
        dragging.OnNext(Unit.Default);
    }

    public void Drop()
    {
        LocalPosition = ArrangedPosition;
        
        dropping.OnNext(Unit.Default);
    }

    public void Arrange(Vector3 atLocalPosition, float withRotationAngle, int andIndex)
    {
        ArrangedPosition = LocalPosition = atLocalPosition;
        RotationAngle = withRotationAngle;
        Index = andIndex;
        
        arrangement.OnNext(Unit.Default);
    }

    public void Move(Vector3 toLocalPosition, CardMoveType withType)
    {
        LocalPosition = toLocalPosition;

        movement.OnNext(withType);
    }

    public void Bounce()
    {
        bouncing.OnNext(Unit.Default);
    }

    public virtual void Flip(CardFace toFace)
    {
        face = toFace;
        
        flipping.OnNext(toFace);
    }

    public void Rotate(float toAngle)
    {
        RotationAngle = toAngle;
        
        rotation.OnNext(Unit.Default);
    }
    
    public void Fade(float toAlphaValue)
    {
        fading.OnNext(toAlphaValue);
    }

    public void Tint(Color withColor, float byFactor)
    {
        tinting.OnNext((withColor, byFactor));
    }
    
    public void Fog(Color withColor, float byFactor)
    {
        fogging.OnNext((withColor, byFactor));
    }

    public virtual void Destroy()
    {
        Bond?.Release(this);

        destruction.OnNext(Unit.Default);
        destruction.OnCompleted();
    }
    
    protected virtual void Strike(int withValue)
    {
        Value -= withValue;
    }
}