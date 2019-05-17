using Zenject;

public class PirateCardController : CardController
{
    public class Factory : PlaceholderFactory<PirateCard, PirateCardView, PirateCardController>
    {
    }
    
    private readonly PirateCard model;
    private readonly PirateCardView view;
    
    public PirateCardController(PirateCard model, PirateCardView view) : base(model, view)
    {
        this.model = model;
        this.view = view;
    }
}