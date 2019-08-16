using Zenject;

public class MerchantCard : Card
{
    public const int CreditMultiplierForMatchingSuit = 3;
    
    private MerchantCard(ICardModel model, IMerchantCardView view) 
        : base(model, view)
    {
        view.Multiplier = CreditMultiplierForMatchingSuit;
    }
    
    public new class Factory : PlaceholderFactory<ICardModel, IMerchantCardView, MerchantCard>
    {
    }
}