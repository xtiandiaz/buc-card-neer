using Zenject;

public class MerchantCardController : CardController
{
    public class Factory : PlaceholderFactory<IMerchantCard, IMerchantCardView, MerchantCardController>
    {
    }
    
    private readonly IMerchantCard model;
    private readonly IMerchantCardView view;
    
    public MerchantCardController(IMerchantCard model, IMerchantCardView view) : base(model, view)
    {
        this.model = model;
        this.view = view;
    }

    public override void Initialize()
    {
        base.Initialize();

        view.Fixation = model.Fixation;
    }
}