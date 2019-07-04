using System;
using UniRx;
using Zenject;

public interface IClashingController : IInitializable, IDisposable
{
    IObservable<Unit> WhenSeaClashed { get; }
}

public class ClashingController : IClashingController
{
    private readonly Subject<Unit> seaClashing = new Subject<Unit>();
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    
    private readonly IBoardingController boardingController;
    private readonly ICardMatcher matcher;
    private readonly ISea sea;

    private ClashingController(
        IBoardingController boardingController,
        ICardMatcher matcher,
        ISea sea
        )
    {
        this.boardingController = boardingController;
        this.matcher = matcher;
        this.sea = sea;
    }

    public IObservable<Unit> WhenSeaClashed => seaClashing;
    
    public void Initialize()
    {
        disposables.Add(sea.WhenReleasedSupply
            .Subscribe(slot => slot.Lock()));
        
        disposables.Add(boardingController.WhenBoarded
            .Merge(matcher.WhenMatched)
            .SelectMany(sea.Clash())
            .Do(unit =>
            {
                seaClashing.OnNext(unit);
                sea.Unlock();
            })
            .Subscribe());
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}