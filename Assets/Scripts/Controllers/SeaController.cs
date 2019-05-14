using System;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

public interface ISeaController : IInitializable, IDisposable
{
}

public class SeaController : ISeaController
{
    public class Factory : PlaceholderFactory<ISea, ISeaView, SeaController>
    {
    }

    private const int FeedCountPerSlot = 3;
    private const float ProjectionDurationInSeconds = 1f;
    private static readonly TimeSpan DealingInterval = TimeSpan.FromSeconds(0.1);
    
    private readonly ISea model;
    private readonly ISeaView view;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private SeaController(ISea model, ISeaView view)
    {
        this.model = model;
        this.view = view;
    }
    
    [Inject]
    public void Initialize()
    {
        model.AssignProviders();
        
        disposables.Add(Observable.Timer(TimeSpan.Zero, DealingInterval)
            .Take(model.Slots.Length * FeedCountPerSlot)
            .Do(i => model.Slots[i % model.Slots.Length].Consume(1))
            .Subscribe());
        
        disposables.Add(model.Slots
            .Select(slot => slot.WhenEmptied.Select(_ => slot))
            .Merge()
            .SelectMany(slot => Observable.Timer(TimeSpan.Zero, DealingInterval)
                .Take(FeedCountPerSlot)
                .Do(_ => slot.Consume(1)))
            .Subscribe());
        
        disposables.Add(model.WhenToggledProjection
            .Subscribe(isProjected => view.ToggleProjection(isProjected, ProjectionDurationInSeconds)));
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}