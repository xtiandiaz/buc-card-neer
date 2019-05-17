using UnityEngine;

public interface IPirateCard : ICard
{
    int LootMultiplier { get; }
}

[CreateAssetMenu(menuName = "Game/Card/Pirate")]
public class PirateCard : Card, IPirateCard
{
    [SerializeField] [Range(2, 4)] private int lootMultiplier;
    
    public override CardType Type => CardType.Pirate;
    public int LootMultiplier => lootMultiplier;

    public override bool CanMatch(ICard withOther, ISlot fromSlot)
    {
        if (!IsBoarded)
            return false;
        
        if (!(withOther is IResourceCard resourceCard) || !resourceCard.IsBoarded)
            return false;

        return (resourceCard.ResourceType & ResourceType.Weapon) != 0;
    }

    public override void Match(ICard withOther)
    {
        if (!(withOther is IResourceCard resourceCard) || (resourceCard.ResourceType & ResourceType.Weapon) == 0) 
            return;
        
        Value -= withOther.Value;

        if (Value <= 0)
            playerStats.Coins += withOther.OriginalValue;
        
        withOther.Destroy();
    }
}