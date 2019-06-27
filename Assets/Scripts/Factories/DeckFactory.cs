using System;
using System.Collections.Generic;
using UniRx;
using Zenject;

public interface IDeckFactory : IFactory<IDeckModel, IDeck>, IDisposable
{
}

public class DeckFactory : IDeckFactory
{
    private readonly Deck.Factory controllerFactory;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private DeckFactory(
        Deck.Factory controllerFactory
        )
    {
        this.controllerFactory = controllerFactory;
    }
    
    public IDeck Create(IDeckModel fromModel)
    {
        var cardData = new List<ICardModel>();
        
        cardData.AddRange(fromModel.Pirates);
        cardData.AddRange(fromModel.Merchants);
        cardData.AddRange(fromModel.Inspectors);
        cardData.AddRange(fromModel.Items);
        cardData.AddRange(fromModel.Tools);
        cardData.AddRange(fromModel.Monsters);
        
        if (fromModel.ShouldShuffleOnInit)
            cardData.Shuffle();
        
        var controller = controllerFactory.Create(cardData);
        
        disposables.Add(controller);
        
        return controller;
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}