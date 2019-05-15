using UnityEngine;

[CreateAssetMenu(fileName = "CardPirate", menuName = "Game/Card/Pirate", order = 1)]
public class CardPirate : Card
{
    public override CardType Type => CardType.Pirate;

    public override bool CanMatch(ICard withOther, ISlot fromSlot)
    {
        if (!(withOther is ICardResource resourceCard) || !resourceCard.IsBoarded)
            return false;

        if (IsBoarded)
        {
            return (resourceCard.ResourceType & ResourceType.WeaponMelee) != 0;
        }
        
        return (resourceCard.ResourceType & ResourceType.WeaponArtillery) != 0;
    }

    public override void Match(ICard withOther)
    {
        if (withOther is ICardResource resourceCard && (resourceCard.ResourceType & ResourceType.Weapon) != 0)
        {
            Value -= withOther.Value;
            withOther.Destroy();
        }
    }
}