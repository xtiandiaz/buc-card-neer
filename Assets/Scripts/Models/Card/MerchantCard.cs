using Zenject;

public class MerchantCard : Card
{
    public class Factory : PlaceholderFactory<MerchantCard>
    {
    }

    private MerchantCard() : base(CardType.Merchant)
    {
    }
    
    public override CardType InteractionMask { get; } = CardType.Item | CardType.Pirate | CardType.Inspector;
}