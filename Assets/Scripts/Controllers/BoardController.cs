using System;
using Zenject;
using UniRx;

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
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private BoardController(IBoard model, IBoardView view)
    {
        this.model = model;
        this.view = view;
    }

    [Inject]
    public void Initialize()
    {
    }

    public void Dispose()
    {
        disposables?.Dispose();
    }
}