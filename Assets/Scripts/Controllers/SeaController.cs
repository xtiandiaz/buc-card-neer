using System;
using UniRx;
using Zenject;

public interface ISeaController
{
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

    [Inject] private IDealingManager dealingManager;

    private SeaController(ISea model, ISeaView view)
    {
        this.model = model;
        this.view = view;
    }
    
    [Inject]
    private void Initialize()
    {
        disposables.Add(
            model.UpdatedProjectionState
                .Subscribe(isProjected => view.ToggleProjection(isProjected, ProjectionDurationInSeconds, 0.65f)));
        
        disposables.Add(model.Dealing
            .SelectMany(fromDeck => dealingManager.Deal(fromDeck, 3, TimeSpan.FromSeconds(0.1), model.Slots))
            .Subscribe());
    }

    public void Dispose()
    {
        disposables?.Dispose();
    }
}