using System;
using UniRx;
using UnityEngine;

public interface IPirateCard : ICard, ILootCarrier
{
    IObservable<Unit> WhenDefeated { get; }
}

[CreateAssetMenu(menuName = "Game/Card/Pirate")]
public class PirateCard : Card, IPirateCard
{
    private readonly Subject<Unit> defeat = new Subject<Unit>();

    [SerializeField] [Range(1, 4)] private int lootMultiplier;
    
    public override CardType Type => CardType.Pirate;
    public bool IsDead => Value <= 0;

    public IObservable<Unit> WhenDefeated => defeat;

    public override bool CanMatch(ICard withOther)
    {
        return withOther is IResourceCard resourceCard && (resourceCard.ResourceType & ResourceType.WeaponMelee) != 0;
    }

    public override void Match(ICard withOther)
    {
        if (!(withOther is IResourceCard resourceCard) || (resourceCard.ResourceType & ResourceType.WeaponMelee) == 0) 
            return;
        
        Hit(withOther.Value);

        withOther.Destroy();
    }

    public override bool CanClash(ICard other)
    {
        return (other.Type & CardType.Merchant) != 0;
    }

    public override bool CanBeImpacted()
    {
        return true;
    }

    public int GetLoot()
    {
        return OriginalValue;
    }

    public override void Hit(int withValue)
    {
        base.Hit(withValue);

        if (Value <= 0)
        {
            defeat.OnNext(Unit.Default);
            defeat.OnCompleted();
        }
    }
}