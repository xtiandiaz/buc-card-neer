using System;
using UniRx;
using UnityEngine;

[Flags]
public enum CardType
{
    Player     = 1 << 0,
    Item       = 1 << 1,
    Foe        = 1 << 2,
    Merchant   = 1 << 3,
    Treasure   = 1 << 4
}

public interface ICard
{
    CardType Type { get; }
    Sprite FrontFace { get; }
    Sprite BackFace { get; }
    CardType InteractionMask { get; }
    SlotType SlotMask { get; }
    
    Vector3 Position { get; set; }
    
    IObservable<Vector3> PositionChanged { get; }
    IObservable<Unit> Picked { get; }
    IObservable<Unit> Dropped { get; }
    IObservable<Unit> Flipped { get; }
    
    void Initialize();
    void Pick();
    void Drop();
    void Flip();
}

public abstract class Card : ScriptableObject, ICard
{
    [SerializeField] private Sprite frontFace;
    [SerializeField] private Sprite backFace;
    
    private readonly ReactiveProperty<Vector3> position = new ReactiveProperty<Vector3>();
    private readonly Subject<Unit> picked = new Subject<Unit>();
    private readonly Subject<Unit> dropped = new Subject<Unit>();
    private readonly Subject<Unit> flipped = new Subject<Unit>();

    public abstract SlotType SlotMask { get; }
    public CardType Type { get; private set; }
    public Sprite FrontFace => frontFace;
    public Sprite BackFace => backFace;
    public CardType InteractionMask => Type;

    public Vector3 Position
    {
        get => position.Value;
        set => position.Value = value;
    }

    public IObservable<Vector3> PositionChanged => position;
    public IObservable<Unit> Picked => picked;
    public IObservable<Unit> Dropped => dropped;
    public IObservable<Unit> Flipped => flipped;
    
    public abstract void Initialize();

    protected void Initialize(CardType withType)
    {
        Type = withType;
    }

    public void Pick()
    {
        picked.OnNext(Unit.Default);
    }

    public void Drop()
    {
        dropped.OnNext(Unit.Default);
    }

    public void Flip()
    {
        flipped.OnNext(Unit.Default);
    }
}