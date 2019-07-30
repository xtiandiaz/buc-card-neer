using System;
using UniRx;
using UnityEngine;
using Zenject;

public interface IGameStatus
{
    IObservable<int> UndealtCardCount { get; }
    IObservable<Unit> WhenLost { get; }
    IObservable<int> WhenWon { get; }

    void Reset();
}

public interface IGameController : IGameStatus, IInitializable, IDisposable
{
}

public class GameController : IGameController
{
    private readonly Subject<Unit> losing = new Subject<Unit>();
    private readonly Subject<int> winning = new Subject<int>();
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    
    private readonly IAppController appController;
    private readonly ISupplyController supplyController;
    private readonly IClashingController clashingController;
    private readonly ICardHost cardHost;
    private readonly ICardDealer cardDealer;
    private readonly IDeck deck;
    private readonly IBoardModel boardModel;
    private readonly IShip ship;
    private readonly IPlayerCard player;
    private readonly IAudioManager audioManager;
    private readonly ISea sea;

    public GameController(
        IAppController appController,
        ISupplyController supplyController,
        IClashingController clashingController,
        ICardHost cardHost,
        ICardDealer cardDealer,
        IShip ship,
        IPlayerCard player,
        IAudioManager audioManager,
        IDeck deck,
        IBoardModel boardModel
        )
    {
        this.appController = appController;
        this.supplyController = supplyController;
        this.clashingController = clashingController;
        this.cardHost = cardHost;
        this.cardDealer = cardDealer;
        this.ship = ship;
        this.player = player;
        this.audioManager = audioManager;
        this.deck = deck;
        this.boardModel = boardModel;
    }

    public IObservable<int> UndealtCardCount => deck.CardCount;
    public IObservable<Unit> WhenLost => losing;
    public IObservable<int> WhenWon => winning;
    
    public void Initialize()
    {
        disposables.Add(cardHost.Lodge(player, ship.Helm)
            .SelectMany(_ => supplyController.Supply()
                .DoOnSubscribe(() => audioManager.Play(AudioEventKey.GameAssemble)))
            .DelaySubscription(TimeSpan.FromSeconds(0.5f))
            .Subscribe());
        
        disposables.Add(player.WhenDestroyed
            .Do(_ => audioManager.Play(AudioEventKey.CardAvatarDeath))
            .Merge(player.WhenBankrupt)
            .Take(1)
            .Delay(TimeSpan.FromSeconds(0.5))
            .Subscribe(_ =>
            {
                OnGameEnded();
                
                audioManager.Play(AudioEventKey.GameLose);
                
                losing.OnNext(Unit.Default);
            }));
        
        disposables.Add(cardDealer.ActiveCardCount
            .SkipUntil(supplyController.WhenSuppliedFirstTime)
            .Where(count => count < boardModel.MaxCardsInSupply && cardDealer.IsThereDeadlock())
            .Take(1)
            .Delay(TimeSpan.FromSeconds(0.5))
            .Subscribe(_ =>
            {
                OnGameEnded();
                
                audioManager.Play(AudioEventKey.GameWin);
                
                winning.OnNext(player.Coins);
            }));
    }

    public void Reset()
    {
        appController.Reload();
    }

    public void Dispose()
    {
        losing.Dispose();
        disposables.Dispose();
    }

    private void OnGameEnded()
    {
        clashingController.Dispose();
        supplyController.Dispose();
    }
}