using System;
using UniRx;
using UnityEngine;

[Flags]
public enum CardType
{
    Player             = 1 << 0,
    Pirate             = 1 << 1,
    Merchant           = 1 << 2,
    Inspector          = 1 << 3,
    Food               = 1 << 4,
    Artifact           = 1 << 5,
    Gem                = 1 << 6,
    WeaponMelee        = 1 << 7,
    WeaponRanged       = 1 << 8,
    Medicine           = 1 << 9,
                         
    Resource           = Food | Gem | Artifact | WeaponMelee | WeaponRanged | Medicine,
    Agent              = Pirate | Merchant | Inspector
}

public enum CardFace
{
    Front,
    Back
}

public interface ICard : IComparable<ICard>
{
    int Value { get; set; }
    int OriginalValue { get; }
    int Index { get; }
    string Name { get; }
    CardType Type { get; }
    Vector3 LocalPosition { get; }
    Sprite FrontFace { get; }
    Sprite BackFace { get; }
    ICardBond Bond { get; }
    bool IsBoarded { get; set; }
    bool IsStored { get; set; }
    DateTimeOffset BindingTimestamp { get; }
    
    IObservable<int> ValueAsObservable { get; }
    IObservable<Direction> WhenClashed { get; }
    IObservable<Unit> WhenImpacted { get; }
    IObservable<Transform> WhenBound { get; }
    IObservable<CardArrangementMode> WhenArranged { get; }
    IObservable<Unit> WhenPicked { get; }
    IObservable<Unit> WhenDragged { get; }
    IObservable<Unit> WhenDropped { get; }
    IObservable<CardFace> WhenFaceChanged { get; }
    IObservable<CardFace> WhenFlipped { get; }
    IObservable<float> WhenFaded { get; }
    IObservable<(Color, float)> WhenTinted { get; }
    IObservable<(Color, float)> WhenFogged { get; }
    IObservable<Unit> WhenDestroyed { get; }

    bool CanMatch(ICard withOther);
    void Match(ICard withOther);
    bool CanClash(ICard other);
    void Clash(ICard other, Direction withDirection);
    bool CanBeImpacted();
    void Impact(int withValue);
    void Bind(ICardBond withBond);
    void Pick();
    void Drag(Vector3 byDeltaPosition);
    void Drop();
    void Arrange(Vector3 atLocalPosition, int withIndex, CardArrangementMode andMode);
    void Flip(CardFace toFace);
    void Fade(float toAlphaValue);
    void Tint(Color withColor, float byFactor);
    void Fog(Color withColor, float byFactor);
    void Destroy();
}

public abstract class Card : ScriptableObject, ICard
{
    [SerializeField] protected IntReactiveProperty value = new IntReactiveProperty();

    private readonly Subject<Direction> clashing = new Subject<Direction>();
    private readonly Subject<Unit> impacting = new Subject<Unit>();
    private readonly Subject<Transform> binding = new Subject<Transform>();
    private readonly Subject<CardArrangementMode> arranging = new Subject<CardArrangementMode>();
    private readonly Subject<Unit> picking = new Subject<Unit>();
    private readonly Subject<Unit> dragging = new Subject<Unit>();
    private readonly Subject<Unit> dropping = new Subject<Unit>();
    private readonly Subject<CardFace> facing = new Subject<CardFace>();
    private readonly Subject<CardFace> flipping = new Subject<CardFace>();
    private readonly Subject<float> fading = new Subject<float>();
    private readonly Subject<(Color, float)> tinting = new Subject<(Color, float)>();
    private readonly Subject<(Color, float)> fogging = new Subject<(Color, float)>();
    private readonly Subject<Unit> destruction = new Subject<Unit>();

    [SerializeField] private Sprite frontFace;
    [SerializeField] private Sprite backFace;
    private CardFace face;
    private float arrangedDepth;

    public abstract CardType Type { get; }
    public int OriginalValue { get; private set; }
    public int Index { get; private set; }
    public string Name => name;
    public Sprite FrontFace => frontFace;
    public Sprite BackFace => backFace;
    public Vector3 LocalPosition { get; private set; }
    public ICardBond Bond { get; private set; }
    public bool IsBoarded { get; set; }
    public bool IsStored { get; set; }
    public DateTimeOffset BindingTimestamp { get; private set; }

    public int Value
    {
        get => value.Value;
        set => this.value.Value = Mathf.Max(value, 0);
    }

    public CardFace Face
    {
        get => face;
        set
        {
            face = value;
            facing.OnNext(value);
        }
    }

    public IObservable<int> ValueAsObservable => value;
    public IObservable<Direction> WhenClashed => clashing;
    public IObservable<Unit> WhenImpacted => impacting;
    public IObservable<Transform> WhenBound => binding;
    public IObservable<CardArrangementMode> WhenArranged => arranging;
    public IObservable<Unit> WhenPicked => picking;
    public IObservable<Unit> WhenDragged => dragging;
    public IObservable<Unit> WhenDropped => dropping;
    public IObservable<CardFace> WhenFaceChanged => facing.DistinctUntilChanged();
    public IObservable<CardFace> WhenFlipped => flipping.DistinctUntilChanged();
    public IObservable<float> WhenFaded => fading;
    public IObservable<(Color, float)> WhenTinted => tinting;
    public IObservable<(Color, float)> WhenFogged => fogging;
    public IObservable<Unit> WhenDestroyed => destruction;

    protected virtual void Awake()
    {
        OriginalValue = Value;
    }

    public abstract bool CanMatch(ICard withOther);

    public abstract void Match(ICard withOther);

    public abstract bool CanClash(ICard other);

    public void Clash(ICard other, Direction withDirection)
    {
        other.Value--;

        clashing.OnNext(withDirection);
    }

    public abstract bool CanBeImpacted();

    public virtual void Impact(int withValue)
    {
        Hit(withValue);

        impacting.OnNext(Unit.Default);
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
        dropping.OnNext(Unit.Default);
    }

    public void Arrange(Vector3 atLocalPosition, int withIndex, CardArrangementMode andMode)
    {
        LocalPosition = atLocalPosition;
        Index = withIndex;
        
        arranging.OnNext(andMode);
    }

    public virtual void Flip(CardFace toFace)
    {
        face = toFace;
        flipping.OnNext(toFace);
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
    
    public virtual int CompareTo(ICard other)
    {
        throw new NotImplementedException();
    }

    protected virtual void Hit(int withValue)
    {
        Value -= withValue;
    }
}