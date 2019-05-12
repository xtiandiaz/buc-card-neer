using System;
using System.Linq;
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

    private const int FeedCountPerSlot = 3;
    private const float ProjectionDurationInSeconds = 1f;
    
    private readonly ISea model;
    private readonly ISeaView view;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private SeaController(ISea model, ISeaView view)
    {
        this.model = model;
        this.view = view;
    }
    
    [Inject]
    private void Initialize()
    {
        model.AssignProviders();
        
        disposables.Add(Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(0.1))
            .Take(model.Slots.Length * FeedCountPerSlot)
            .Do(i => model.Slots[i % model.Slots.Length].Consume(1))
            .Subscribe());
        
        disposables.Add(model.WhenToggledProjection
            .Subscribe(isProjected => view.ToggleProjection(isProjected, ProjectionDurationInSeconds)));
    }

    public void Dispose()
    {
        disposables?.Dispose();
    }
}