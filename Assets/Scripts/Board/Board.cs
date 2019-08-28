using Zenject;

public interface IBoard
{
    ISea Sea { get; }
    IShip Ship { get; }
}

public class Board : IBoard
{
    private readonly ISea sea;
    private readonly IShip ship;

    private Board(
        ISea sea,
        IShip ship
    )
    {
        this.sea = sea;
        this.ship = ship;
    }

    public ISea Sea => sea;
    public IShip Ship => ship;

    public class Factory : PlaceholderFactory<ISea, IShip, Board>
    {
    }
}