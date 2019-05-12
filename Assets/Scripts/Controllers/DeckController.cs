using System;
using UniRx;
using Zenject;

public class DeckController : IDisposable
{
    public class Factory : PlaceholderFactory<IDeck, DeckController>
    {
    }
    
    private readonly IDeck model;
    private readonly ICardFactory cardFactory;
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    
    private DeckController(
        IDeck model,
        ICardFactory cardFactory
        )
    {
        this.model = model;
        this.cardFactory = cardFactory;
    }

    [Inject]
    private void Initialize()
    {
        model.Shuffle();
        
        disposables.Add(model.WhenProvided.Subscribe(card => cardFactory.Create(card)));
    }

    public void Dispose()
    {
        disposables?.Dispose();
    }
}