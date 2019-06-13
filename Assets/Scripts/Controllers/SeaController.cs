using System;
using System.Linq;
using UniRx;
using Zenject;

public interface ISeaController : IInitializable, IDisposable
{
}

public class SeaController : ISeaController
{
    public class Factory : PlaceholderFactory<ISea, ISeaView, SeaController>
    {
    }
    
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

        disposables.Add(model.Slots
            .Select(slot => slot.WhenReleased.Do(_ => slot.Lock()))
            .Merge()
            .Subscribe());
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}