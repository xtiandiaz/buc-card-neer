using UniRx;
using Zenject;

public class MerchantCardController : CardController
{
    public class Factory : PlaceholderFactory<IMerchantCard, IMerchantCardView, MerchantCardController>
    {
    }
    
    private readonly IMerchantCard model;
    private readonly IMerchantCardView view;
    private readonly IPlayerCard player;

    public MerchantCardController(
        IMerchantCard model, 
        IMerchantCardView view,
        IPlayerCard player
        ) 
        : base(model, view)
    {
        this.model = model;
        this.view = view;
        this.player = player;
    }

    public override void Initialize()
    {
        base.Initialize();

        view.Fixation = model.Fixation;
        
        disposables.Add(model.WhenBought.Subscribe(resCard => player.Sell(resCard, model)));
    }
}