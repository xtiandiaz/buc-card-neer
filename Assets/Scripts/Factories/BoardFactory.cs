using System;
using UniRx;
using Zenject;

public interface IBoardFactory : IFactory<IBoard>, IDisposable
{
}

public class BoardFactory : IBoardFactory
{
    private readonly Board.Factory modelFactory;
    private readonly BoardView.Factory viewFactory;
    private readonly BoardController.Factory controllerFactory;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private BoardFactory(
        Board.Factory modelFactory,
        BoardView.Factory viewFactory,
        BoardController.Factory controllerFactory
        )
    {
        this.modelFactory = modelFactory;
        this.viewFactory = viewFactory;
        this.controllerFactory = controllerFactory;
    }
    
    public IBoard Create()
    {
        var model = modelFactory.Create();
        var view = viewFactory.Create();
        
        disposables.Add(controllerFactory.Create(model, view));
        
        return model;
    }

    public void Dispose()
    {
        disposables?.Dispose();
    }
}