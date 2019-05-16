using UnityEngine;

[CreateAssetMenu(menuName = "Game/Card/Pirate")]
public class PirateCard : Card
{
    public override CardType Type => CardType.Pirate;

    public override bool CanMatch(ICard withOther, ISlot fromSlot)
    {
        if (!(withOther is IResourceCard resourceCard) || !resourceCard.IsBoarded)
            return false;

        if (IsBoarded)
        {
            return (resourceCard.ResourceType & ResourceType.WeaponMelee) != 0;
        }
        
        return (resourceCard.ResourceType & ResourceType.WeaponArtillery) != 0;
    }

    public override void Match(ICard withOther)
    {
        if (withOther is IResourceCard resourceCard && (resourceCard.ResourceType & ResourceType.Weapon) != 0)
        {
            Value -= withOther.Value;
            withOther.Destroy();
        }
    }
}