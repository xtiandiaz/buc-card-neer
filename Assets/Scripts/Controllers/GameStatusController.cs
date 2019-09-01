using System;
using UniRx;
using Zenject;

public interface IGameStatus : IInitializable, IDisposable
{
    bool PlayerDidStashItem { get; set; }
    bool PlayerDidStashTool { get; set; }
    bool DidSupplyOnce { get; set; }
    
    IObservable<int> UndealtCardCount { get; }
    IObservable<Unit> WhenPlayerLost { get; }
    IObservable<int> WhenPlayerWon { get; }
    
    IObservable<Unit> WhenPlayerShot { get; set; }
    IObservable<Unit> WhenPlayerUnlockedAndHandledCard { get; set; }
    IObservable<Unit> WhenPlayerAttackedOnBoard { get; set; }
}

public interface IGameStatusController : IGameStatus, IInitializable, IDisposable
{
}

public class GameStatusController : IGameStatusController
{
    private readonly IAppController appController;
    private readonly IPlayerCard player;
    private readonly IDeck deck;
    private readonly IDealingController dealer;
    private readonly IBoardModel boardModel;
    private readonly Subject<Unit> losing = new Subject<Unit>();
    private readonly Subject<int> winning = new Subject<int>();
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private IObservable<CardType> whenCardRevealed;
    private IObservable<CardType> whenCardStashed;
    
    public bool PlayerDidStashItem { get; set; }
    public bool PlayerDidStashTool { get; set; }
    public bool DidSupplyOnce { get; set; }

    public IObservable<int> UndealtCardCount => deck.CardCount;
    public IObservable<Unit> WhenPlayerLost => losing;
    public IObservable<int> WhenPlayerWon => winning;

    public IObservable<Unit> WhenPlayerShot { get; set; }
    public IObservable<CardType> WhenPlayerBoardedCard { get; set; }
    public IObservable<CardType> WhenPlayerBoardedAndHandledCard { get; set; }
    public IObservable<Unit> WhenPlayerUnlockedAndHandledCard { get; set; }
    public IObservable<Unit> WhenPlayerAttackedOnBoard { get; set; }

    private GameStatusController(
        IAppController appController,
        IPlayerCard player, 
        IDeck deck,
        IDealingController dealer,
        IBoardModel boardModel
        )
    {
        this.appController = appController;
        this.player = player;
        this.deck = deck;
        this.dealer = dealer;
        this.boardModel = boardModel;
    }

    public void Initialize()
    {
        disposables.Add(
            player.WhenDestroyed
                .Take(1)
                .Subscribe(losing));
        
        disposables.Add(dealer.ActiveCardCount
            .Where(count => DidSupplyOnce && count < boardModel.MaxCardsInSupply && dealer.IsThereDeadlock())
            .Take(1)
            .Subscribe(_ => winning.OnNext(player.Coins)));
    }
    
    public void Dispose()
    {
        winning.Dispose();
        losing.Dispose();
        
        disposables.Dispose();
    }
}