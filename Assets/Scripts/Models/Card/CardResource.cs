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
    Money               = CardType.Money
}

public interface IResourceCard : ICard
{
    ResourceType ResourceType { get; }
    Sprite Item { get; }
    ISuit Suit { get; }
    bool IsLoot { get; }
    bool IsTreasure { get; }
    bool IsPurchase { get; }
    bool IsPurchasable { get; }
    bool IsConsumable { get; }
    
    IObservable<Unit> WhenPurchased { get; } 
    IObservable<Unit> WhenSold { get; } 
    IObservable<Unit> WhenConsumed { get; } 

    bool Purchase();
    bool Sell();
    bool Consume();
}

[CreateAssetMenu(fileName = "CardResource", menuName = "Game/Card/Resource", order = 1)]
public class CardResource : Card, IResourceCard
{
    private readonly Subject<Unit> purchasing = new Subject<Unit>();
    private readonly Subject<Unit> selling = new Subject<Unit>();
    private readonly Subject<Unit> consumption = new Subject<Unit>();

    [SerializeField] private Sprite item;
    [SerializeField] private Suit suit;
    [SerializeField] private Suit isLoot;
    [SerializeField] private bool isTreasure;
    private IPlayerStats playerStats;

    public override CardType Type => (CardType) ResourceType;
    public ResourceType ResourceType => suit.ResourceType;
    public Sprite Item => item;
    public ISuit Suit => suit;
    public bool IsLoot => isLoot;
    public bool IsTreasure => isTreasure;
    public bool IsPurchase { get; private set; }
    public bool IsPurchasable => !IsLoot && !IsTreasure && !IsPurchase;
    public bool IsConsumable => (!IsPurchasable || IsPurchase) && (Type & CardType.Food) != 0;

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
        
        if (playerStats.Coins < Value)
            return false;
        
        playerStats.Coins -= Value;
        IsPurchase = true;
        
        purchasing.OnNext(Unit.Default);
        purchasing.OnCompleted();

        return true;
    }

    public bool Sell()
    {
        playerStats.Coins += Value;
        
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