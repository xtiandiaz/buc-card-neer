using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;

public interface IBoard
{
}

public class Board : IBoard
{
    public class Factory : PlaceholderFactory<int, int, Board>
    {
    }
    
    private readonly int cols;
    private readonly int rows;
    private readonly CardSlot.Factory tileFactory;
    private readonly GameSettings settings;
    private readonly Dictionary<Tuple<int, int>, CardSlot> tiles = new Dictionary<Tuple<int, int>, CardSlot>();

    public List<CardSlot> PlaySlots { get; set; }
    public List<CardSlot> StashSlots { get; set; }
    public CardSlot PlayerSlot { get; set; }
    
    public Board(
        int cols, 
        int rows, 
        CardSlot.Factory tileFactory,
        GameSettings settings
        )
    {
        this.cols = cols;
        this.rows = rows;
        this.tileFactory = tileFactory;
        this.settings = settings;

        Build();
    }

    public IEnumerable<CardSlot> Tiles => tiles.Select(t => t.Value);

    public CardSlot this[int xCoordinate, int yCoordinate]
    {
        get
        {
            var coordinate = Tuple.Create(xCoordinate, yCoordinate);
            return !tiles.ContainsKey(coordinate) ? null : tiles[coordinate];
        }
    }

    private void Build()
    {
        var halfCols = cols / 2;
        var halfRows = rows / 2;
        var colsM1 = cols - 1;
        var rowsM1 = rows - 1;
        
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < cols; j++)
            {
                var coords = new Coordinates(-halfCols + j, -halfRows + i);
                //var newTile = tileFactory.Create(coords);

                //tiles[Tuple.Create(coords.x, coords.y)] = newTile;
            }
        }

        /*foreach (var valuePair in tiles)
        {
            var tile = valuePair.Value;
            var tileCoords = tile.Coordinates;

            tile.Neighbors[Direction.Up] = this[tileCoords.x, tileCoords.y + 1];
            tile.Neighbors[Direction.Right] = this[tileCoords.x + 1, tileCoords.y];
            tile.Neighbors[Direction.Down] = this[tileCoords.x, tileCoords.y - 1];
            tile.Neighbors[Direction.Left] = this[tileCoords.x - 1, tileCoords.y];
        }*/

        /*var nonPlayerTiles = tiles.Where(t => !t.Value.IsCenter && !t.Value.IsEdge)
            .Select(t => t.Value).ToList();

        for (var i = 0; i < settings.LockedTileCount; i++)
        {
            nonPlayerTiles.PopRandomItem().Lock();
        }*/

    }
}