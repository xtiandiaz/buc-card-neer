using UnityEngine;

public interface IPirateCard : ICard
{
    int LootMultiplier { get; }
}

[CreateAssetMenu(menuName = "Game/Card/Pirate")]
public class PirateCard : Card, IPirateCard
{
    [SerializeField] [Range(1, 4)] private int lootMultiplier;
    
    public override CardType Type => CardType.Pirate;
    public int LootMultiplier => lootMultiplier;

    public override bool CanMatch(ICard withOther, ISlot fromSlot)
    {
        return withOther is IResourceCard resourceCard && (resourceCard.ResourceType & ResourceType.WeaponMelee) != 0;
    }

    public override void Match(ICard withOther)
    {
        if (!(withOther is IResourceCard resourceCard) || (resourceCard.ResourceType & ResourceType.WeaponMelee) == 0) 
            return;
        
        Value -= withOther.Value;

        withOther.Destroy();
    }

    public override bool CanClash(ICard other)
    {
        return (other.Type & CardType.Merchant) != 0;
    }
}