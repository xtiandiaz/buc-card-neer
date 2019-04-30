using System;
using UniRx;
using Zenject;

public interface ISeaController
{
    void Initialize();
}

public class SeaController : ISeaController, IDisposable
{
    public class Factory : PlaceholderFactory<ISea, ISeaView, SeaController>
    {
    }

    private const float ProjectionDurationInSeconds = 1f;
    
    private readonly ISea model;
    private readonly ISeaView view;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private SeaController(ISea model, ISeaView view)
    {
        this.model = model;
        this.view = view;
    }

    public void Initialize()
    {
        disposables.Add(
            model.UpdatedProjectionState
                .Subscribe(isProjected => view.ToggleProjection(isProjected, ProjectionDurationInSeconds)));
    }

    public void Dispose()
    {
        disposables?.Dispose();
    }
}