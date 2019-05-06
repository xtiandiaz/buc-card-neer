using Zenject;

public class CardPlayerController : CardController
{
    public class Factory : PlaceholderFactory<CardPlayer, CardPlayerView, CardPlayerController>
    {
    }
    
    private readonly CardPlayer model;
    private readonly CardPlayerView view;
    
    public CardPlayerController(CardPlayer model, CardPlayerView view) : base(model, view)
    {
        this.model = model;
        this.view = view;
    }
}