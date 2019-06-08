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
    public override CardType Type => CardType.Pirate;
    public bool IsDead => Value <= 0;

    public IObservable<Unit> WhenDefeated => WhenAttacked.Where(_ => Value <= 0).AsUnitObservable();

    public override bool CanMatch(ICard withOther)
    {
        return withOther is IResourceCard resourceCard && (resourceCard.ResourceType & ResourceType.WeaponMelee) != 0;
    }

    public override void Match(ICard withOther)
    {
        if (!(withOther is IResourceCard resourceCard) || (resourceCard.ResourceType & ResourceType.WeaponMelee) == 0) 
            return;
        
        Strike(withOther.Value, PlayerAttackType.Blow);

        withOther.Destroy();
    }

    public override bool CanClash(ICard other)
    {
        return (other.Type & CardType.Merchant) != 0;
    }

    public override bool CanBeStruck()
    {
        return true;
    }

    public int GetLoot()
    {
        return OriginalValue;
    }
}