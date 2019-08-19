using Zenject;

public interface IMerchantCard
{
    void Resuit(ISuitModel withModel);
}

public class MerchantCard : Card, IMerchantCard
{
    public const int CreditMultiplierForMatchingSuit = 3;
    
    private readonly IMerchantCardView view;
    
    private MerchantCard(ICardModel model, IMerchantCardView view) 
        : base(model, view)
    {
        this.view = view;
        view.Multiplier = CreditMultiplierForMatchingSuit;
    }

    public void Resuit(ISuitModel withModel)
    {
        if (withModel == null)
            return;;
        
        Suit = withModel;
    }
    
    public new class Factory : PlaceholderFactory<ICardModel, IMerchantCardView, MerchantCard>
    {
    }
}