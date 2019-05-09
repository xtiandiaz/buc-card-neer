using System;
using UniRx;
using UnityEngine;

[Flags]
public enum CardType
{
    Player             = 1 << 0,
    Pirate             = 1 << 1,
    Merchant           = 1 << 2,
    Food               = 1 << 3,
    Artifact           = 1 << 4,
    Gem                = 1 << 5,
    WeaponMelee        = 1 << 6,
    WeaponArtillery    = 1 << 7,
    Money              = 1 << 8,
                         
    Resource           = Food | Gem | Artifact | WeaponMelee | WeaponArtillery
}

public enum CardFace
{
    Front,
    Back
}

public interface ICard
{
    int Value { get; set; }
    int Index { get; }
    string Name { get; }
    CardType Type { get; }
    Vector3 Position { get; }
    bool IsVisible { get; set; }
    Sprite FrontFace { get; }
    Sprite BackFace { get; }
    
    IObservable<int> Worth { get; }
    IObservable<Unit> WhenPicked { get; }
    IObservable<Vector3> WhenDragged { get; }
    IObservable<Unit> WhenDropped { get; }
    IObservable<Unit> WhenArranged { get; }
    IObservable<CardFace> WhenFaceChanged { get; }
    IObservable<CardFace> WhenFlipped { get; }
    IObservable<bool> WhenVisibilityChanged { get; }
    IObservable<float> WhenFaded { get; }
    IObservable<(Color, float)> WhenTinted { get; }
    IObservable<(Color, float)> WhenFogged { get; }
    IObservable<Unit> WhenDestroyed { get; }

    bool CanMatch(ICard withOther, ISlot fromSlot);
    void Match(ICard withOther);
    void Bind(ICardBind to);
    void Pick();
    void Drag(Vector3 toPosition);
    void Drop();
    void Arrange(Vector3 atPosition, int withIndex);
    void Flip(CardFace toFace);
    void Fade(float toAlphaValue);
    void Tint(Color withColor, float byFactor);
    void Fog(Color withColor, float byFactor);
    ICard Clone();
    void Destroy();
}

public abstract class Card : ScriptableObject, ICard
{
    [SerializeField] protected IntReactiveProperty value = new IntReactiveProperty();
    
    private readonly ReactiveProperty<bool> isVisible = new ReactiveProperty<bool>(true);
    private readonly BehaviorSubject<Unit> arranging = new BehaviorSubject<Unit>(Unit.Default);
    private readonly Subject<Unit> picking = new Subject<Unit>();
    private readonly Subject<Vector3> dragging = new Subject<Vector3>();
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
    private ICardBind bind;
    private float arrangedDepth;

    public abstract CardType Type { get; }
    
    public int Value
    {
        get => value.Value;
        set => this.value.Value = Mathf.Max(value, 0);
    }

    public int Index { get; private set; }
    public string Name => name;
    
    public CardFace Face
    {
        get => face;
        set
        {
            face = value;
            facing.OnNext(value);
        }
    }

    public Vector3 Position { get; private set; }

    public bool IsVisible
    {
        get => isVisible.Value;
        set => isVisible.Value = value;
    }
    
    public Sprite FrontFace => frontFace;
    public Sprite BackFace => backFace;

    public IObservable<int> Worth => value;
    public IObservable<Unit> WhenArranged => arranging;
    public IObservable<Unit> WhenPicked => picking;
    public IObservable<Vector3> WhenDragged => dragging;
    public IObservable<Unit> WhenDropped => dropping;
    public IObservable<CardFace> WhenFaceChanged => facing.DistinctUntilChanged();
    public IObservable<CardFace> WhenFlipped => flipping.DistinctUntilChanged();
    public IObservable<bool> WhenVisibilityChanged => isVisible;
    public IObservable<float> WhenFaded => fading;
    public IObservable<(Color, float)> WhenTinted => tinting;
    public IObservable<(Color, float)> WhenFogged => fogging;
    public IObservable<Unit> WhenDestroyed => destruction;

    public abstract bool CanMatch(ICard withOther, ISlot fromSlot);

    public abstract void Match(ICard withOther);

    public void Bind(ICardBind to)
    {
        if (to == bind || to == null)
            return;
        
        bind?.Release(this);
        bind = to;
    }

    public void Pick()
    {
        Position = new Vector3(Position.x, Position.y, 0);
        
        picking.OnNext(Unit.Default);
    }
    
    public void Drag(Vector3 toPosition)
    {
        Position = toPosition = new Vector3(toPosition.x, toPosition.y, Position.z);
        
        dragging.OnNext(toPosition);
    }

    public void Drop()
    {
        Position = new Vector3(Position.x, Position.y, arrangedDepth);
        
        dropping.OnNext(Unit.Default);
    }

    public void Arrange(Vector3 atPosition, int withIndex)
    {
        Position = atPosition;
        Index = withIndex;
        arrangedDepth = atPosition.z;
        
        arranging.OnNext(Unit.Default);
    }

    public void Flip(CardFace toFace)
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

    public ICard Clone()
    {
        return Instantiate(this);
    }

    public void Destroy()
    {
        bind?.Release(this);
        
        destruction.OnNext(Unit.Default);
        destruction.OnCompleted();
    }
}