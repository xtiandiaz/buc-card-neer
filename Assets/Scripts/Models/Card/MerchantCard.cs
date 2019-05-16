using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Card/Merchant")]
public class MerchantCard : Card
{
    [SerializeField] private ResourceFixation[] fixations;
    
    public override CardType Type => CardType.Merchant;

    public override bool CanMatch(ICard withOther, ISlot fromSlot)
    {
        return withOther is IResourceCard resourceCard && resourceCard.WasPurchased;
    }

    public override void Match(ICard withOther)
    {
        if (withOther is IResourceCard resourceCard && resourceCard.WasPurchased)
            resourceCard.Sell(
                fixations.FirstOrDefault(f => (f.Suit.ResourceType & resourceCard.ResourceType) != 0)?.Degree ?? 1);
    }
}