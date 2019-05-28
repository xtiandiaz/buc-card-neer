using System;
using Zenject;
using UniRx;
using UnityEngine;

public interface IBoardController : IInitializable, IDisposable
{
}

public class BoardController : IBoardController
{
    public class Factory : PlaceholderFactory<IBoard, IBoardView, BoardController>
    {
    }
    
    private readonly IBoard model;
    private readonly IBoardView view;
    private readonly ISea sea;
    private readonly IMoveListener moveListener;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private BoardController(
        IBoard model, 
        IBoardView view,
        ISea sea,
        IMoveListener moveListener
        )
    {
        this.model = model;
        this.view = view;
        this.sea = sea;
        this.moveListener = moveListener;
    }

    [Inject]
    public void Initialize()
    {
        disposables.Add(moveListener.WhenMoved
            .Subscribe(_ =>
            {
                Debug.Log("Player Moved!");
                
                sea.Clash();
            }));
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}