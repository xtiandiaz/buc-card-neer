using System;
using UniRx;
using Zenject;

public interface IGameStatus
{
    bool PlayerDidStoreItem { get; set; }
    bool PlayerDidStoreTool { get; set; }
    bool DidSupplyOnce { get; set; }
    
    IObservable<int> UndealtCardCount { get; }
    IObservable<Unit> WhenLost { get; }
    IObservable<int> WhenWon { get; }

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
    
    public bool PlayerDidStoreItem { get; set; }
    public bool PlayerDidStoreTool { get; set; }
    public bool DidSupplyOnce { get; set; }

    public IObservable<int> UndealtCardCount => deck.CardCount;
    public IObservable<Unit> WhenLost => losing;
    public IObservable<int> WhenWon => winning;

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
            .Subscribe(winning));
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