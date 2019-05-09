using UnityEngine;

[CreateAssetMenu(fileName = "CardMerchant", menuName = "Game/Card/Merchant", order = 1)]
public class CardMerchant : Card
{
    public override CardType Type => CardType.Merchant;

    public override bool CanMatch(ICard withOther, ISlot fromSlot)
    {
        return withOther is IResourceCard resourceCard && resourceCard.IsPurchase;
    }

    public override void Match(ICard withOther)
    {
        if (withOther is IResourceCard resourceCard && resourceCard.IsPurchase)
            resourceCard.Sell();
    }
}