using UniRx;
using Zenject;

public class PirateCardController : CardController
{
    public class Factory : PlaceholderFactory<IPirateCard, IPirateCardView, PirateCardController>
    {
    }
    
    private readonly IPirateCard model;
    private readonly IPirateCardView view;

    [Inject] private IPlunderer plunderer;
    
    public PirateCardController(IPirateCard model, IPirateCardView view) : base(model, view)
    {
        this.model = model;
        this.view = view;
    }

    public override void Initialize()
    {
        base.Initialize();
        
        disposables.Add(model.WhenDestroyed.Subscribe(_ => plunderer.Plunder(model)));
    }
}