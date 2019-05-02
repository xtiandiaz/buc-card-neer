using UnityEngine;

[CreateAssetMenu(fileName = "CardPirate", menuName = "Game/Card/Pirate", order = 1)]
public class CardPirate : Card
{
    [SerializeField] private int hitPoints;

    public override CardType Type => CardType.Pirate;
    public override CardType InteractionMask => CardType.Resource;
    public override int Value => hitPoints;
}