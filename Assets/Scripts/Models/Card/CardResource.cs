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
    bool IsTreasure { get; }
    bool WasPaidFor { get; }
    
    IObservable<Unit> WhenPurchased { get; } 

    bool Purchase();
    bool Sell();
}

[CreateAssetMenu(fileName = "CardResource", menuName = "Game/Card/Resource", order = 1)]
public class CardResource : Card, IResourceCard
{
    private readonly Subject<Unit> purchasing = new Subject<Unit>();

    [SerializeField] private Sprite item;
    [SerializeField] private Suit suit;
    [SerializeField] private bool isTreasure;
    private IPlayerStats playerStats;

    public override CardType Type => (CardType) ResourceType;
    public ResourceType ResourceType => suit.ResourceType;
    public Sprite Item => item;
    public ISuit Suit => suit;
    public bool IsTreasure => isTreasure;
    public bool WasPaidFor { get; private set; }

    public IObservable<Unit> WhenPurchased => purchasing;

    [Inject]
    private void Construct(IPlayerStats playerStats)
    {
        this.playerStats = playerStats;
    }

    public override bool CanMatch(ICard withOther)
    {
        // No other Card can be matched on top of a Resource
        return false;
    }

    public override void Match(ICard withOther)
    {
    }

    public bool Purchase()
    {
        if (playerStats.Coins < Value)
            return false;
        
        playerStats.Coins -= Value;
        WasPaidFor = true;
        
        purchasing.OnNext(Unit.Default);

        return true;
    }

    public bool Sell()
    {
        playerStats.Coins += Value;
        
        Destroy();

        return true;
    }
}