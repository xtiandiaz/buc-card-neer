using System;
using System.Collections.Generic;
using UniRx;
using Zenject;

public interface IDeckFactory : IFactory<IDeck, IDeck>, IDisposable
{
}

public class DeckFactory : IDeckFactory
{
    private readonly DeckController.Factory controllerFactory;
    private readonly List<IDeck> decks;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private DeckFactory(
        DeckController.Factory controllerFactory
        )
    {
        this.controllerFactory = controllerFactory;
    }
    
    public IDeck Create(IDeck forModel)
    {        
        disposables.Add(controllerFactory.Create(forModel));
        
        return forModel;
    }

    public void Dispose()
    {
        disposables?.Dispose();
    }
}