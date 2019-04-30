using UnityEngine;

[CreateAssetMenu(fileName = "CardFoe", menuName = "Game/Card/Foe", order = 1)]
public class CardFoe : Card
{
    [SerializeField] private int hitPoints;

    public override CardType InteractionMask => CardType.Resource;
    public override int Value => hitPoints;

    public override void Initialize()
    {
        base.Initialize(CardType.Foe);
    }
}