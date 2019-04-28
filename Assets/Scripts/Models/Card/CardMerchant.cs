using UnityEngine;

[CreateAssetMenu(fileName = "CardMerchant", menuName = "Game/Card Merchant", order = 1)]
public class CardMerchant : Card
{
    public override SlotType SlotMask => SlotType.Boarding;
    
    public override void Initialize()
    {
        base.Initialize(CardType.Merchant);
    }
}