using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CardMerchant", menuName = "Game/Card/Merchant", order = 1)]
public class CardMerchant : Card
{
    [SerializeField] private ResourceFixation[] fixations;
    
    public override CardType Type => CardType.Merchant;

    public override bool CanMatch(ICard withOther, ISlot fromSlot)
    {
        return withOther is ICardResource resourceCard && resourceCard.WasPurchased;
    }

    public override void Match(ICard withOther)
    {
        if (withOther is ICardResource resourceCard && resourceCard.WasPurchased)
            resourceCard.Sell(
                fixations.FirstOrDefault(f => (f.Suit.ResourceType & resourceCard.ResourceType) != 0)?.Degree ?? 1);
    }
}