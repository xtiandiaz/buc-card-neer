using System;
using UniRx;
using Zenject;

public interface IBoardingController : IInitializable, IDisposable
{
    IObservable<Unit> WhenBoarded { get; }
}

public class BoardingController : IBoardingController
{
    private readonly ICardHost cardHost;
    private readonly Subject<Unit> boarding = new Subject<Unit>();
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    
    private readonly IShip ship;
    private readonly IAudioManager audioManager;

    private BoardingController(
        ICardHost cardHost,
        IShip ship,
        IAudioManager audioManager
        )
    {
        this.cardHost = cardHost;
        this.ship = ship;
        this.audioManager = audioManager;
    }

    public IObservable<Unit> WhenBoarded => boarding;

    public void Initialize()
    {
        disposables.Add(ship.Plank.WhenLodged
            .Where(card => !card.IsBoarded)
            .Do(card =>
            {
                card.IsBoarded = true;
                
                if (card.IsMonster)
                    audioManager.Play(AudioEventKey.CardBoardMonster);
                else
                    audioManager.Play(AudioEventSwitchKey.CardBoard, card.Type);
            })
            .SelectMany(card => card.IsLocked ? Observable.ReturnUnit() : Handle(card))
            .Do(boarding.OnNext)
            .Subscribe());

        disposables.Add(ship.Plank.WhenLodged
            .Where(card => card.IsLocked)
            .SelectMany(card => card.WhenUnlocked
                .ContinueWith(_ => card.Drop())
                .Delay(TimeSpan.FromSeconds(0.25))
                .ContinueWith(_ => Handle(card))
                .Do(boarding.OnNext))
            .Subscribe());
    }

    public void Dispose()
    {
        boarding.Dispose();
        disposables.Dispose();
    }

    private IObservable<Unit> Handle(ICard card)
    {
        return card.Reveal()
            .DoOnSubscribe(() =>
            {
                if (card.IsMonster)
                    audioManager.Play(AudioEventKey.CardRevealMonster);
                else
                    audioManager.Play(AudioEventSwitchKey.CardReveal, card.Type);
            })
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
                audioManager.Play(AudioEventSwitchKey.CardStash, card.Type);
            });
    }
}