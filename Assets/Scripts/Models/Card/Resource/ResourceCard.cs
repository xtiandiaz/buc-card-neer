using System;
using UniRx;
using UnityEngine;

public interface IResourceCard : ICard
{
    ResourceType ResourceType { get; }
    Sprite Container { get; }
    Sprite Item { get; }
    ISuit Suit { get; }
    int LockValue { get; }
    bool IsWrapped { get; }
    bool WasLocked { get; }
    bool IsLocked { get; }

    IObservable<int> WhenLockValueChanged { get; }
    IObservable<Unit> WhenUnlocked { get; }
    
    void Unwrap();
    void Unlock();
}

[CreateAssetMenu(fileName = "CardResource", menuName = "Game/Card/Resource", order = 1)]
public class ResourceCard : Card, IResourceCard
{
    [SerializeField] private Sprite container;
    [SerializeField] private Sprite item;
    [SerializeField] private Suit suit;
    [SerializeField] private IntReactiveProperty lockValue = new IntReactiveProperty();

    public override CardType Type => (CardType) ResourceType;
    public ResourceType ResourceType => suit.ResourceType;
    public Sprite Container => container;
    public Sprite Item => item;
    public ISuit Suit => suit;
    public bool IsWrapped { get; private set; } = true;
    public bool WasLocked { get; private set; }
    public bool IsLocked => LockValue > 0;

    public int LockValue
    {
        get => lockValue.Value;
        private set
        {
            lockValue.Value = value;
            WasLocked |= value > 0;
        }
    }

    public IObservable<int> WhenLockValueChanged => lockValue;
    public IObservable<Unit> WhenUnlocked => lockValue.Where(x => x <= 0).Take(1).AsUnitObservable();
    
    protected  void Awake()
    {
        WasLocked = IsLocked;
    }

    public override bool CanReflect(ICard other)
    {
        return (ResourceType & ResourceType.Trap) != 0;
    }

    public override bool CanMatch(ICard withOther)
    {
        return IsLocked && (withOther.Type & CardType.WeaponMelee) != 0;
    }

    public override void Match(ICard withOther)
    {
        if (IsLocked && (withOther.Type & CardType.WeaponMelee) == 0) 
            return;
        
        Strike(withOther.Value, PlayerAttackType.Blow);

        withOther.Value = 0;
    }

    public override bool CanClash(ICard other)
    {
        return false;
    }

    public override bool CanBeStruck()
    {
        return IsLocked;
    }

    public void Unwrap()
    {
        IsWrapped = false;
        
        Flip(CardFace.Front);
    }

    public void Unlock()
    {
        LockValue = 0;
    }

    protected override void Strike(int withValue)
    {
        LockValue -= withValue;
    }
}