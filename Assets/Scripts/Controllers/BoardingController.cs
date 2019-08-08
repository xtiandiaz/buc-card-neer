using System;
using UniRx;
using Zenject;

public interface IBoardingController : IInitializable, IDisposable
{
    IObservable<CardType> WhenCardStashed { get; }
    IObservable<CardType> WhenCardRevealed { get; }
}

public class BoardingController : IBoardingController
{
    private readonly ICardHost cardHost;
    private readonly Subject<CardType> boarding = new Subject<CardType>();
    private readonly Subject<CardType> boardingAndHandling = new Subject<CardType>();
    private readonly Subject<CardType> revealing = new Subject<CardType>();
    private readonly Subject<CardType> stashing = new Subject<CardType>();
    private readonly Subject<Unit> unlockingAndHandling = new Subject<Unit>();
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    
    private readonly IShip ship;
    private readonly ISea sea;
    private readonly IGameStatus gameStatus;

    private BoardingController(
        ICardHost cardHost,
        IShip ship,
        ISea sea,
        IGameStatus gameStatus
        )
    {
        this.cardHost = cardHost;
        this.ship = ship;
        this.sea = sea;

        this.gameStatus = gameStatus;
        this.gameStatus.WhenPlayerBoardedCard = boarding;
        this.gameStatus.WhenPlayerBoardedAndHandledCard = boardingAndHandling;
        this.gameStatus.WhenPlayerUnlockedAndHandledCard = unlockingAndHandling;
    }

    public IObservable<CardType> WhenCardStashed => stashing;
    public IObservable<CardType> WhenCardRevealed => revealing;

    public void Initialize()
    {
        disposables.Add(ship.Plank.WhenLodged
            .Where(card => !card.IsBoarded)
            .Do(card =>
            {
                sea.Lock();
                
                card.IsBoarded = true;
                
                boarding.OnNext(card.IsMonster ? CardType.Monster : card.Type);
            })
            .SelectMany(card => (card.IsLocked ? Observable.ReturnUnit() : Handle(card))
                .Do(_ => boardingAndHandling.OnNext(card.IsMonster ? CardType.Monster : card.Type)))
            .Subscribe());

        disposables.Add(ship.Plank.WhenLodged
            .Where(card => card.IsLocked)
            .SelectMany(card => card.WhenUnlocked
                .ContinueWith(_ => card.DropAsObservable())
                .Delay(TimeSpan.FromSeconds(0.25))
                .ContinueWith(_ => Handle(card))
                .Do(unlockingAndHandling.OnNext))
            .Subscribe());
    }

    public void Dispose()
    {
        boarding.Dispose();
        boardingAndHandling.Dispose();
        unlockingAndHandling.Dispose();
        revealing.Dispose();
        stashing.Dispose();
        disposables.Dispose();
    }

    private IObservable<Unit> Handle(ICard card)
    {
        return card.Reveal()
            .DoOnSubscribe(() => revealing.OnNext(card.IsMonster ? CardType.Monster : card.Type))
            .ContinueWith(_ => card.IsResource
                ? Observable.Timer(TimeSpan.FromSeconds(0.25)).ContinueWith(Store(card))
                : Observable.Empty<Unit>())
            .LastOrDefault();
    }

    private IObservable<Unit> Store(ICard card)
    {
        return cardHost.Lodge(ship.Plank, ship.GetStash(card.Type))
            .Do(_ =>
            {
                card.IsStored = true;

                gameStatus.PlayerDidStoreItem |= card.IsItem;
                gameStatus.PlayerDidStoreTool |= card.IsTool;

                stashing.OnNext(card.Type);
            });
    }
}