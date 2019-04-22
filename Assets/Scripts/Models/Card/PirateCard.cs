using UniRx;
using Zenject;

public class PirateCard : Card
{
    public class Factory : PlaceholderFactory<PirateCard>
    {
    }
    
    protected PirateCard() : base(CardType.Pirate)
    {
    }
    
    public override CardType InteractionMask { get; } = CardType.Item | CardType.Merchant | CardType.Inspector;
}