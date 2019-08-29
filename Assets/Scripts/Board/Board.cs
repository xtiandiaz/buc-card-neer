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
    private readonly IBoardModel model;

    private Board(
        ISea sea,
        IShip ship,
        IBoardModel model
    )
    {
        this.sea = sea;
        this.ship = ship;
        this.model = model;
    }

    public ISea Sea => sea;
    public IShip Ship => ship;

    public class Factory : PlaceholderFactory<ISea, IShip, IBoardModel, Board>
    {
    }
}