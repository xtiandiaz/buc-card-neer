using System;
using UniRx;
using Zenject;

public interface IClashingController : IInitializable, IDisposable
{
    IObservable<Unit> WhenRoundCompleted { get; }
}

public class ClashingController : IClashingController
{
    private readonly Subject<Unit> roundCompletion = new Subject<Unit>();
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    
    private readonly ICardMatcher matcher;
    private readonly ICardShooter shooter;
    private readonly ICardForwarder forwarder;
    private readonly ISea sea;
    private readonly IGameStatus gameStatus;

    private ClashingController(
        ICardMatcher matcher,
        ICardShooter shooter,
        ICardForwarder forwarder,
        ISea sea,
        IGameStatus gameStatus
        )
    {
        this.matcher = matcher;
        this.shooter = shooter;
        this.forwarder = forwarder;
        this.sea = sea;
        this.gameStatus = gameStatus;
    }

    public IObservable<Unit> WhenRoundCompleted => roundCompletion;
    
    public void Initialize()
    {
        disposables.Add(gameStatus.WhenPlayerBoardedAndHandledCard.AsUnitObservable()
            .Merge(
                gameStatus.WhenPlayerUnlockedAndHandledCard,
                matcher.WhenMatched, 
                shooter.WhenRestored, 
                forwarder.WhenForwarded)
            .Delay(TimeSpan.FromSeconds(0.25))
            .SelectMany(sea.Clash()
                .LastOrDefault()
                .Do(roundCompletion.OnNext))
            .Subscribe());
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}