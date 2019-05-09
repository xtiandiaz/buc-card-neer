using UnityEngine;
using Zenject;

public class GameController : IInitializable
{
    private readonly IBoard board;

    public GameController(IBoard board)
    {
        this.board = board;
    }

    public void Initialize()
    {
        Application.targetFrameRate = 50;
        
        board.Populate();
    }
}