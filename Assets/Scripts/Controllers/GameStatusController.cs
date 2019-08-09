using System;
using UniRx;
using Zenject;

public interface IGameStatus
{
    bool PlayerDidStashItem { get; set; }
    bool PlayerDidStashTool { get; set; }
    bool DidSupplyOnce { get; set; }
    
    IObservable<int> UndealtCardCount { get; }
    IObservable<Unit> WhenLost { get; }
    IObservable<int> WhenWon { get; }
    
    IObservable<Unit> WhenPlayerShot { get; set; }
    IObservable<Unit> WhenPlayerUnlockedAndHandledCard { get; set; }
    IObservable<Unit> WhenPlayerAttackedOnBoard { get; set; }
    IObservable<Unit> WhenPlayerConfronted { get; set; }

    void Reset();
}

public interface IGameStatusController : IGameStatus, IInitializable, IDisposable
{
}

public class GameStatusController : IGameStatusController
{
    private readonly IAppController appController;
    private readonly IPlayerCard player;
    private readonly IDeck deck;
    private readonly ICardDealer cardDealer;
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
    public IObservable<Unit> WhenLost => losing;
    public IObservable<int> WhenWon => winning;

    public IObservable<Unit> WhenPlayerShot { get; set; }
    public IObservable<CardType> WhenPlayerBoardedCard { get; set; }
    public IObservable<CardType> WhenPlayerBoardedAndHandledCard { get; set; }
    public IObservable<Unit> WhenPlayerUnlockedAndHandledCard { get; set; }
    public IObservable<Unit> WhenPlayerAttackedOnBoard { get; set; }
    public IObservable<Unit> WhenPlayerConfronted { get; set; }

    private GameStatusController(
        IAppController appController,
        IPlayerCard player, 
        IDeck deck,
        ICardDealer cardDealer,
        IBoardModel boardModel
        )
    {
        this.appController = appController;
        this.player = player;
        this.deck = deck;
        this.cardDealer = cardDealer;
        this.boardModel = boardModel;
    }

    public void Initialize()
    {
        disposables.Add(
            player.WhenDestroyed
                .Take(1)
                .Subscribe(losing));
        
        disposables.Add(cardDealer.ActiveCardCount
            .Where(count => DidSupplyOnce && count < boardModel.MaxCardsInSupply && cardDealer.IsThereDeadlock())
            .Take(1)
            .Subscribe(_ => winning.OnNext(player.Coins)));
    }

    public void Reset()
    {
        appController.Reload();
    }
    
    public void Dispose()
    {
        winning.Dispose();
        losing.Dispose();
        
        disposables.Dispose();
    }
}