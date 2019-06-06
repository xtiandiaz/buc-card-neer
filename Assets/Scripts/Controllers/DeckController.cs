using System;
using UniRx;
using UnityEngine;
using Zenject;

public interface IDeckController : IInitializable, IDisposable
{}

public class DeckController : IDeckController 
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
    public void Initialize()
    {
        disposables.Add(model.WhenProvided.Subscribe(card => cardFactory.Create(card)));
    }

    public void Dispose()
    {
        disposables?.Dispose();
    }
}