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
    int IndexInSlot { get; }
    string Name { get; }
    CardType Type { get; }
    CardType InteractionMask { get; }
    CardFace Face { get; }
    Vector3 Position { get; }
    bool IsVisible { get; set; }
    Sprite FrontFace { get; }
    Sprite BackFace { get; }
    
    IObservable<int> Worth { get; }
    IObservable<Unit> Arranging { get; }
    IObservable<Unit> Picking { get; }
    IObservable<Unit> Dragging { get; }
    IObservable<Vector3> Dropping { get; }
    IObservable<CardFace> Facing { get; }
    IObservable<bool> Visibility { get; }
    IObservable<float> Fading { get; }
    IObservable<(Color, float)> Tinting { get; }
    IObservable<(Color, float)> Fogging { get; }
    IObservable<Unit> Destruction { get; }

    bool DoesConsume(ICard other);
    void Lodge(ISlot inSlot);
    void Pick();
    void Drag(Vector3 toPosition);
    void Drop(Vector3 atPosition);
    void Flip(CardFace to);
    void Arrange(Vector3 atPosition, int withIndex, int fromTotal, ICardArrangementSettings andWithSettings);
    void Fade(float toAlphaValue);
    void Tint(Color withColor, float byFactor);
    void Fog(Color withColor, float byFactor);
    ICard Clone();
    void Destroy();
}

public abstract class Card : ScriptableObject, ICard
{
    [SerializeField] protected IntReactiveProperty value = new IntReactiveProperty();
    [SerializeField] private Sprite frontFace;
    [SerializeField] private Sprite backFace;
    private ISlot slot;
    
    private readonly ReactiveProperty<CardFace> facing = new ReactiveProperty<CardFace>();
    private readonly ReactiveProperty<bool> visibility = new ReactiveProperty<bool>(true);
    private readonly Subject<Unit> arranging = new Subject<Unit>();
    private readonly Subject<Unit> picking = new Subject<Unit>();
    private readonly Subject<Unit> dragging = new Subject<Unit>();
    private readonly Subject<Vector3> dropping = new Subject<Vector3>();
    private readonly Subject<float> fading = new Subject<float>();
    private readonly Subject<(Color, float)> tinting = new Subject<(Color, float)>();
    private readonly Subject<(Color, float)> fogging = new Subject<(Color, float)>();
    private readonly Subject<Unit> destruction = new Subject<Unit>();

    public abstract CardType Type { get; }
    public abstract CardType InteractionMask { get; }
    
    public virtual int Value
    {
        get => value.Value;
        set => this.value.Value = value;
    }

    public int IndexInSlot { get; private set; }
    public string Name => name;
    public CardFace Face => facing.Value;
    public Vector3 Position { get; private set; }

    public bool IsVisible
    {
        get => visibility.Value;
        set => visibility.Value = value;
    }
    
    public Sprite FrontFace => frontFace;
    public Sprite BackFace => backFace;

    public IObservable<int> Worth => value;
    public IObservable<Unit> Arranging => arranging;
    public IObservable<Unit> Picking => picking;
    public IObservable<Unit> Dragging => dragging;
    public IObservable<Vector3> Dropping => dropping;
    public IObservable<CardFace> Facing => facing.DistinctUntilChanged();
    public IObservable<bool> Visibility => visibility;
    public IObservable<float> Fading => fading;
    public IObservable<(Color, float)> Tinting => tinting;
    public IObservable<(Color, float)> Fogging => fogging;
    public IObservable<Unit> Destruction => destruction;

    public abstract bool DoesConsume(ICard other);

    public void Lodge(ISlot inSlot)
    {
        if (slot == inSlot)
            return;
        
        slot?.Release(this);
        
        slot = inSlot;
    }

    public void Pick()
    {
        Position = new Vector3(Position.x, Position.y, 0);
        
        picking.OnNext(Unit.Default);
    }
    
    public void Drag(Vector3 toPosition)
    {
        Position = new Vector3(toPosition.x, toPosition.y, Position.z);
        
        dragging.OnNext(Unit.Default);
    }

    public void Drop(Vector3 atPosition)
    {
        Position = atPosition;
        
        dropping.OnNext(atPosition);
    }

    public void Flip(CardFace to)
    {
        facing.Value = to;
    }

    public void Arrange(Vector3 atPosition, int withIndex, int fromTotal, ICardArrangementSettings andWithSettings)
    {
        Position = andWithSettings.Transform(atPosition, withIndex, fromTotal);
        IndexInSlot = withIndex;

        if (andWithSettings.ShouldFog)
            Fog(andWithSettings.FogColor, andWithSettings.FogDamping * withIndex / fromTotal);
        
        arranging.OnNext(Unit.Default);
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
        slot?.Release(this);
        
        destruction.OnNext(Unit.Default);
        destruction.OnCompleted();
    }
}