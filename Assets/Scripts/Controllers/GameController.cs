using UniRx;
using UnityEngine;
using Zenject;

public class GameController : IInitializable
{
    private readonly IBoardFactory boardFactory;
    private readonly IBoardView boardView;

    private IBoard board;

    public GameController(
        IBoardFactory boardFactory,
        IBoardView boardView
    )
    {
        this.boardFactory = boardFactory;
        this.boardView = boardView;
    }

    public void Initialize()
    {
        Application.targetFrameRate = 50;

        board = boardFactory.Create(boardView);

        board.Deal();
    }
}