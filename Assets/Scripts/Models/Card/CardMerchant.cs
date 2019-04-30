using UnityEngine;

[CreateAssetMenu(fileName = "CardMerchant", menuName = "Game/Card/Merchant", order = 1)]
public class CardMerchant : Card
{
    public override CardType InteractionMask => CardType.Resource;
    public override int Value => 1;
    
    public override void Initialize()
    {
        base.Initialize(CardType.Merchant);
    }
}