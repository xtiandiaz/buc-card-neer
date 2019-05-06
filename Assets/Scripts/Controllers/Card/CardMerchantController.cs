using Zenject;

public class CardMerchantController : CardController
{
    public class Factory : PlaceholderFactory<CardMerchant, CardMerchantView, CardMerchantController>
    {
    }
    
    private readonly CardMerchant model;
    private readonly CardMerchantView view;
    
    public CardMerchantController(CardMerchant model, CardMerchantView view) : base(model, view)
    {
        this.model = model;
        this.view = view;
    }
}