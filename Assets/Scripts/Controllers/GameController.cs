using System;
using UniRx;
using UnityEngine;
using Zenject;

public class GameController : IInitializable, IDisposable
{
    private readonly IBoard board;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    public GameController(IBoard board)
    {
        this.board = board;
    }

    public void Initialize()
    {
        Application.targetFrameRate = 50;

        board.Populate();
    }

    public void Dispose()
    {
        disposables?.Dispose();
    }
}