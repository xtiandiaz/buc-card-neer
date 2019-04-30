using System;
using UniRx;
using UnityEngine;

[Flags]
public enum CardType
{
    Player     = 1 << 0,
    Resource   = 1 << 1,
    Foe        = 1 << 2,
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
    string Name { get; }
    CardType Type { get; }
    CardType InteractionMask { get; }
    CardFace Face { get; }
    Vector3 LocalPosition { get; set; }
    bool IsVisible { get; set; }
    Sprite FrontFace { get; }
    Sprite BackFace { get; }
    
    IObservable<Unit> Picked { get; }
    IObservable<Vector3> Dropped { get; }
    IObservable<Transform> Lodged { get; }
    IObservable<CardFace> ChangedFace { get; }
    IObservable<bool> BecameVisible { get; }
    IObservable<Vector3> ChangedLocalPosition { get; }
    
    void Initialize();
    void Pick();
    void Drop(Vector3 atPosition);
    void Flip(CardFace to);
    void Lodge(Transform inTransform);
    ICard Clone();
}

public abstract class Card : ScriptableObject, ICard
{
    [SerializeField] private Sprite frontFace;
    [SerializeField] private Sprite backFace;
    
    private readonly ReactiveProperty<Vector3> localPosition = new ReactiveProperty<Vector3>();
    private readonly ReactiveProperty<CardFace> face = new ReactiveProperty<CardFace>();
    private readonly ReactiveProperty<bool> isVisible = new ReactiveProperty<bool>(true);
    private readonly Subject<Unit> picked = new Subject<Unit>();
    private readonly Subject<Vector3> dropped = new Subject<Vector3>();
    private readonly Subject<Transform> lodged = new Subject<Transform>();

    public abstract int Value { get; }
    public abstract CardType InteractionMask { get; }
    public string Name => name;
    public CardFace Face => face.Value;
    public CardType Type { get; private set; }
    
    public Vector3 LocalPosition
    {
        get => localPosition.Value;
        set => localPosition.Value = value;
    }

    public bool IsVisible
    {
        get => isVisible.Value;
        set => isVisible.Value = value;
    }
    
    public Sprite FrontFace => frontFace;
    public Sprite BackFace => backFace;

    public IObservable<Unit> Picked => picked;
    public IObservable<Vector3> Dropped => dropped;
    public IObservable<Transform> Lodged => lodged;
    public IObservable<CardFace> ChangedFace => face.DistinctUntilChanged();
    public IObservable<bool> BecameVisible => isVisible;
    public IObservable<Vector3> ChangedLocalPosition => localPosition;
    
    public abstract void Initialize();

    protected void Initialize(CardType withType)
    {        
        Type = withType;
    }

    public void Pick()
    {
        picked.OnNext(Unit.Default);
    }

    public void Drop(Vector3 atPosition)
    {
        dropped.OnNext(atPosition);
    }

    public void Flip(CardFace to)
    {
        face.Value = to;
    }

    public void Lodge(Transform inTransform)
    {
        lodged.OnNext(inTransform);
    }

    public ICard Clone()
    {
        var clone = Instantiate(this);
        
        clone.Initialize();

        return clone;
    }
}