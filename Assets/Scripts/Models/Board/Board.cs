using Zenject;

public interface IBoard
{
}

public class Board : IBoard
{
    public class Factory : PlaceholderFactory<Board>
    {
    }
    
    private Board()
    {
    }
}