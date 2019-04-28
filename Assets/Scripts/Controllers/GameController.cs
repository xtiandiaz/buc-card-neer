using UniRx;
using UnityEngine;
using Zenject;

public class GameController : IInitializable
{
    private readonly BoardFactory boardFactory;
    private readonly IBoardView boardView;
    private readonly GameSettings settings;

    public GameController(
        BoardFactory boardFactory,
        IBoardView boardView, 
        GameSettings settings
    )
    {
        this.boardFactory = boardFactory;
        this.boardView = boardView;
        this.settings = settings;
    }

    public void Initialize()
    {
        Application.targetFrameRate = 50;

        boardFactory.Create(boardView);
       
    }
}