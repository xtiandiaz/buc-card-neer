using UnityEngine;

[CreateAssetMenu(fileName = "CardMerchant", menuName = "Game/Card/Merchant", order = 1)]
public class CardMerchant : Card
{
    public override int Value => 1;
    public override CardType Type => CardType.Merchant;
    public override CardType InteractionMask => CardType.Resource;
}