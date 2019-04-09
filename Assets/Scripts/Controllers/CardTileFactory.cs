using System;
using Zenject;

public class CardTileFactory : IFactory<Coordinates, Tuple<CardTile, CardTileView>>
{
    private readonly CardTile.Factory cardTileFactory;
    private readonly CardTileView.Factory cardTileViewFactory;

    private CardTileFactory(CardTile.Factory cardTileFactory, CardTileView.Factory cardTileViewFactory)
    {
        this.cardTileFactory = cardTileFactory;
        this.cardTileViewFactory = cardTileViewFactory;
    }
    
    public Tuple<CardTile, CardTileView> Create(Coordinates coordinates)
    {
        var model = cardTileFactory.Create(coordinates);
        var view = cardTileViewFactory.Create(model);

        return Tuple.Create(model, view);
    }
    
    public CardTileView CreateTileView(ICardTile forTile, BoardView inView)
    {
        var tileView = cardTileViewFactory.Create(forTile);
        tileView.transform.SetParent(inView.transform);
        
        return tileView;
    }
}