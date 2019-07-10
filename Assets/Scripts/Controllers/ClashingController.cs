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
    private readonly ICardShooter shooter;
    private readonly ISea sea;

    private ClashingController(
        IBoardingController boardingController,
        ICardMatcher matcher,
        ICardShooter shooter,
        ISea sea
        )
    {
        this.boardingController = boardingController;
        this.matcher = matcher;
        this.shooter = shooter;
        this.sea = sea;
    }

    public IObservable<Unit> WhenSeaClashed => seaClashing;
    
    public void Initialize()
    {
        disposables.Add(sea.WhenReleasedSupply
            .Subscribe(slot => slot.Lock()));
        
        disposables.Add(boardingController.WhenBoarded
            .Merge(matcher.WhenMatched, shooter.WhenShot)
            .Delay(TimeSpan.FromSeconds(0.25))
            .SelectMany(sea.Clash())
            .Do(_ =>
            {
                seaClashing.OnNext(_);
                sea.Unlock();
            })
            .Subscribe());
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}