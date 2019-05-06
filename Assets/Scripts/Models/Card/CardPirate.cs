using UnityEngine;

[CreateAssetMenu(fileName = "CardPirate", menuName = "Game/Card/Pirate", order = 1)]
public class CardPirate : Card
{
    public override CardType Type => CardType.Pirate;
    public override CardType InteractionMask => CardType.Resource;

    public override bool DoesConsume(ICard other)
    {
        return false;
    }
}