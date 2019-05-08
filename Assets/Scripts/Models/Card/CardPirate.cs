using UnityEngine;

[CreateAssetMenu(fileName = "CardPirate", menuName = "Game/Card/Pirate", order = 1)]
public class CardPirate : Card
{
    public override CardType Type => CardType.Pirate;

    public override bool CanMatch(ICard withOther)
    {
        return (withOther.Type & CardType.WeaponArtillery) != 0;
    }

    public override void Match(ICard withOther)
    {
        if ((withOther.Type & CardType.WeaponArtillery) != 0)
        {
            Value -= withOther.Value;
            withOther.Destroy();
        }
    }
}