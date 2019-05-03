using System;
using UniRx;
using UnityEngine;

[Flags]
public enum CardType
{
    Player     = 1 << 0,
    Resource   = 1 << 1,
    Pirate     = 1 << 2,
    Merchant   = 1 << 3
}

public enum CardFace
{
    Front,
    Back
}

public interface ICard
{
    int Value { get; }
    int IndexInSlot { get; }
    string Name { get; }
    CardType Type { get; }
    CardType InteractionMask { get; }
    CardFace Face { get; }
    Vector3 Position { get; }
    bool IsVisible { get; set; }
    Sprite FrontFace { get; }
    Sprite BackFace { get; }
    
    IObservable<Unit> Arranging { get; }
    IObservable<Unit> Picking { get; }
    IObservable<Vector3> Dropping { get; }
    IObservable<CardFace> Facing { get; }
    IObservable<bool> Visibility { get; }
    IObservable<float> Fading { get; }
    IObservable<(Color, float)> Tinting { get; }
    IObservable<(Color, float)> Fogging { get; }
    
    void Pick();
    void Drop(Vector3 atPosition);
    void Flip(CardFace to);
    void Arrange(Vector3 atPosition, int withIndex);
    void Fade(float toAlphaValue);
    void Tint(Color withColor, float byFactor);
    void Fog(Color withColor, float byFactor);
    ICard Clone();
}

public abstract class Card : ScriptableObject, ICard
{
    [SerializeField] private Sprite frontFace;
    [SerializeField] private Sprite backFace;
    
    private readonly ReactiveProperty<CardFace> facing = new ReactiveProperty<CardFace>();
    private readonly ReactiveProperty<bool> visibility = new ReactiveProperty<bool>(true);
    private readonly Subject<Unit> arranging = new Subject<Unit>();
    private readonly Subject<Unit> picking = new Subject<Unit>();
    private readonly Subject<Vector3> dropping = new Subject<Vector3>();
    private readonly Subject<float> fading = new Subject<float>();
    private readonly Subject<(Color, float)> tinting = new Subject<(Color, float)>();
    private readonly Subject<(Color, float)> fogging = new Subject<(Color, float)>();

    public abstract int Value { get; }
    public abstract CardType Type { get; }
    public abstract CardType InteractionMask { get; }
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

    public IObservable<Unit> Arranging => arranging;
    public IObservable<Unit> Picking => picking;
    public IObservable<Vector3> Dropping => dropping;
    public IObservable<CardFace> Facing => facing.DistinctUntilChanged();
    public IObservable<bool> Visibility => visibility;
    public IObservable<float> Fading => fading;
    public IObservable<(Color, float)> Tinting => tinting;
    public IObservable<(Color, float)> Fogging => fogging;

    public void Pick()
    {
        picking.OnNext(Unit.Default);
    }

    public void Drop(Vector3 atPosition)
    {
        dropping.OnNext(atPosition);
    }

    public void Flip(CardFace to)
    {
        facing.Value = to;
    }

    public void Arrange(Vector3 atPosition, int withIndex)
    {
        Position = atPosition;
        IndexInSlot = withIndex;
        
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
}