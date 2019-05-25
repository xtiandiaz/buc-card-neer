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
    private readonly IMoveObservable moveObservable;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private BoardController(
        IBoard model, 
        IBoardView view,
        ISea sea,
        IMoveObservable moveObservable
        )
    {
        this.model = model;
        this.view = view;
        this.sea = sea;
        this.moveObservable = moveObservable;
    }

    [Inject]
    public void Initialize()
    {
        disposables.Add(moveObservable.WhenMoved
            .Subscribe(_ =>
            {
                Debug.Log("Player Moved!");
                
                sea.Clash();
                sea.Unlock(); // For Supply Slots are locked upon release 
            }));
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}