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

    private const int FeedCountPerSlot = 3;
    private static readonly TimeSpan DealingInterval = TimeSpan.FromSeconds(0.1);
    private static readonly TimeSpan ClashingInterval = TimeSpan.FromSeconds(0.5);
    private static readonly TimeSpan SupplyDelay = TimeSpan.FromSeconds(0.75);
    
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

        #region Dealing

        disposables.Add(Observable.Timer(TimeSpan.Zero, DealingInterval)
            .Take(model.Slots.Length * FeedCountPerSlot)
            .Do(i => model.Slots[i % model.Slots.Length].Consume(1))
            .Subscribe());

        disposables.Add(model.Slots
            .Select(slot => slot.WhenEmptied.Select(_ => slot))
            .Merge()
            .Delay(SupplyDelay)
            .SelectMany(slot => Observable.Timer(TimeSpan.Zero, DealingInterval)
                .Take(FeedCountPerSlot)
                .Do(_ => slot.Consume(1)))
            .Subscribe());

        #endregion

        #region Clashing

        disposables.Add(model.WhenClashed
            .SelectMany(_ =>
            {
                var indicesToClash = Enumerable.Range(0, model.Slots.Length).Where(i => model.CanClash(i)).ToArray();
                
                return Observable.Timer(TimeSpan.Zero, ClashingInterval)
                    .Take(indicesToClash.Length)
                    .Do(i => model.Clash(indicesToClash[(int) i]))
                    .DoOnCompleted(model.Unlock); // For Supply Slots are locked upon release
            }) 
            .Subscribe());

        #endregion
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}