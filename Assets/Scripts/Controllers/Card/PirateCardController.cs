using Zenject;

public class PirateCardController : CardController
{
    public class Factory : PlaceholderFactory<PirateCard, CardPirateView, PirateCardController>
    {
    }
    
    private readonly PirateCard model;
    private readonly CardPirateView view;
    
    public PirateCardController(PirateCard model, CardPirateView view) : base(model, view)
    {
        this.model = model;
        this.view = view;
    }
}