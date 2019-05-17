using System;
using System.Reflection;
using UniRx;
using UnityEngine;
using Zenject;

[Flags]
public enum ResourceType
{
    None                = 0,
    Food                = CardType.Food,
    Artifact            = CardType.Artifact,
    Gem                 = CardType.Gem,
    Weapon              = CardType.Weapon,
    
    Item                = Food | Artifact | Gem
}

public interface IResourceCard : ICard
{
    ResourceType ResourceType { get; }
    Sprite Item { get; }
    ISuit Suit { get; }
    int LockValue { get; }
    bool IsAcquired { get; }
    bool IsPurchasable { get; }
    bool IsLocked { get; }
    bool IsConsumable { get; }
    
    IObservable<int> LockValueAsObservable { get; }
    IObservable<Unit> WhenUnlocked { get; }
    IObservable<Unit> WhenPurchased { get; } 
    IObservable<Unit> WhenSold { get; } 
    IObservable<Unit> WhenConsumed { get; } 

    bool Purchase();
    bool Sell(int byFactor);
    bool Consume();
    bool Unlock();
}

[CreateAssetMenu(fileName = "CardResource", menuName = "Game/Card/Resource", order = 1)]
public class ResourceCard : Card, IResourceCard
{
    private readonly Subject<Unit> purchasing = new Subject<Unit>();
    private readonly Subject<Unit> selling = new Subject<Unit>();
    private readonly Subject<Unit> consumption = new Subject<Unit>();

    [SerializeField] private Sprite item;
    [SerializeField] private Suit suit;
    [SerializeField] private IntReactiveProperty lockValue = new IntReactiveProperty();

    public override CardType Type => (CardType) ResourceType;
    public ResourceType ResourceType => suit.ResourceType;
    public Sprite Item => item;
    public ISuit Suit => suit;
    public bool IsAcquired { get; private set; }
    public bool IsPurchasable => !IsAcquired && !IsLocked;
    public bool IsLocked => LockValue > 0;
    public bool IsConsumable => !IsLocked && (Type & CardType.Food) != 0;

    public int LockValue
    {
        get => lockValue.Value;
        set => lockValue.Value = value;
    }

    public IObservable<int> LockValueAsObservable => lockValue;
    public IObservable<Unit> WhenUnlocked => lockValue.Where(x => x <= 0).Take(1).AsSingleUnitObservable();
    public IObservable<Unit> WhenPurchased => purchasing;
    public IObservable<Unit> WhenSold => selling;
    public IObservable<Unit> WhenConsumed => consumption;

    public override bool CanMatch(ICard withOther, ISlot fromSlot)
    {
        if (!IsBoarded || !IsLocked)
            return false;
        
        if (!(withOther is IResourceCard resourceCard) || !resourceCard.IsBoarded)
            return false;
        
        return (resourceCard.ResourceType & ResourceType.Weapon) != 0;
    }

    public override void Match(ICard withOther)
    {
        if (!(withOther is IResourceCard resourceCard) || (resourceCard.ResourceType & ResourceType.Weapon) == 0) 
            return;
        
        LockValue -= withOther.Value;
        
        withOther.Destroy();
    }

    public override void Flip(CardFace toFace)
    {
        if (IsLocked && toFace == CardFace.Front)
            return;
        
        base.Flip(toFace);
    }

    public bool Purchase()
    {
        if (!IsPurchasable)
            return false;
        
        IsAcquired = true;
        
        purchasing.OnNext(Unit.Default);
        purchasing.OnCompleted();

        return true;
    }

    public bool Sell(int byFactor)
    {
        playerStats.Coins += Value * byFactor;
        
        selling.OnNext(Unit.Default);
        selling.OnCompleted();
        
        Destroy();

        return true;
    }

    public bool Consume()
    {
        if (!IsConsumable)
            return false;
        
        var didConsume = true;
        
        switch (ResourceType)
        {
            case ResourceType.Food:

                playerStats.HealthPoints += Value;

                break;
            default:
                didConsume = false;
                break;
        }

        if (didConsume)
        {
            consumption.OnNext(Unit.Default);
            consumption.OnCompleted();
            
            Destroy();
        }

        return didConsume;
    }

    public bool Unlock()
    {
        if (!IsLocked)
            return false;

        playerStats.HealthPoints -= LockValue;
        LockValue = 0;

        return true;
    }
}