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
    WeaponArtillery     = CardType.WeaponArtillery,
    WeaponMelee         = CardType.WeaponMelee,
    Money               = CardType.Money,
    
    Item                = Food | Artifact | Gem, 
    Weapon              = WeaponMelee | WeaponArtillery
}

public interface ICardResource : ICard
{
    ResourceType ResourceType { get; }
    Sprite Item { get; }
    ISuit Suit { get; }
    int LockValue { get; }
    bool WasPurchased { get; }
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
}

[CreateAssetMenu(fileName = "CardResource", menuName = "Game/Card/Resource", order = 1)]
public class CardResource : Card, ICardResource
{
    private readonly Subject<Unit> purchasing = new Subject<Unit>();
    private readonly Subject<Unit> selling = new Subject<Unit>();
    private readonly Subject<Unit> consumption = new Subject<Unit>();

    [SerializeField] private Sprite item;
    [SerializeField] private Suit suit;
    [SerializeField] private IntReactiveProperty lockValue = new IntReactiveProperty();
    private IPlayerStats playerStats;

    public override CardType Type => (CardType) ResourceType;
    public ResourceType ResourceType => suit.ResourceType;
    public Sprite Item => item;
    public ISuit Suit => suit;
    public int LockValue => lockValue.Value;
    public bool WasPurchased { get; private set; }
    public bool IsPurchasable => !WasPurchased && !IsLocked;
    public bool IsLocked => LockValue > 0;
    public bool IsConsumable => !IsLocked && (Type & CardType.Food) != 0;

    public IObservable<int> LockValueAsObservable => lockValue;
    public IObservable<Unit> WhenUnlocked => lockValue.Where(x => x <= 0).Take(1).AsSingleUnitObservable();
    public IObservable<Unit> WhenPurchased => purchasing;
    public IObservable<Unit> WhenSold => selling;
    public IObservable<Unit> WhenConsumed => consumption;

    [Inject]
    private void Construct(IPlayerStats playerStats)
    {
        this.playerStats = playerStats;
    }

    public override bool CanMatch(ICard withOther, ISlot fromSlot)
    {
        // No other Card can be matched on top of a Resource
        return false;
    }

    public override void Match(ICard withOther)
    {
    }

    public bool Purchase()
    {
        if (!IsPurchasable)
            return false;
        
        WasPurchased = true;
        
        purchasing.OnNext(Unit.Default);
        purchasing.OnCompleted();

        return true;
    }

    public bool Sell(int byFactor)
    {
        playerStats.Coins += Value * byFactor;
        
        Destroy();
        
        selling.OnNext(Unit.Default);
        selling.OnCompleted();

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
            Value = 0;
            
            consumption.OnNext(Unit.Default);
            consumption.OnCompleted();
        }

        return didConsume;
    }
}