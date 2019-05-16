using Zenject;

public class MerchantCardController : CardController
{
    public class Factory : PlaceholderFactory<MerchantCard, CardMerchantView, MerchantCardController>
    {
    }
    
    private readonly MerchantCard model;
    private readonly CardMerchantView view;
    
    public MerchantCardController(MerchantCard model, CardMerchantView view) : base(model, view)
    {
        this.model = model;
        this.view = view;
    }
}