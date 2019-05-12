using System.Collections.Generic;
using Zenject;

public interface IDeckFactory : IFactory<IDeck, IDeck>
{
}

public class DeckFactory : IDeckFactory
{
    private readonly DeckController.Factory controllerFactory;
    private readonly List<IDeck> decks;

    private DeckFactory(
        DeckController.Factory controllerFactory
        )
    {
        this.controllerFactory = controllerFactory;
    }
    
    public IDeck Create(IDeck forModel)
    {        
        controllerFactory.Create(forModel);
        
        return forModel;
    }
}