using System;
using UniRx;
using UnityEngine;
using Zenject;

public class DeckController : IDisposable
{
    public class Factory : PlaceholderFactory<IDeck, DeckController>
    {
    }
    
    private readonly IDeck model;
    private readonly CardFactory cardFactory;
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    
    private DeckController(
        IDeck model,
        CardFactory cardFactory
        )
    {
        this.model = model;
        this.cardFactory = cardFactory;
    }

    public void Initialize()
    {
        disposables.Add(
            model.Supplied
                .Subscribe(card => cardFactory.Create(card)));
    }

    public void Dispose()
    {
        disposables?.Dispose();
    }
}