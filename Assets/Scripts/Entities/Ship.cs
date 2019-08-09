using System;
using UniRx;
using Zenject;

public interface IShip : IInitializable, IDisposable
{
    ISlot Helm { get; }
    ISlot Plank { get; }
    ISlot Storage { get; }
    ISlot Mount { get; }
    
    IObservable<Unit> WhenArmed { get; }
    IObservable<CardType> WhenCardBoarded { get; }
    IObservable<CardType> WhenCardRevealed { get; }
    IObservable<CardType> WhenCardStashed { get; }
    IObservable<CardType> WhenCardHacked { get; }
    IObservable<CardType> WhenCardHandled { get; }

    void Lock();
    void Unlock();
    
    IObservable<Unit> ExpressBoard(ICard card);
}

public class Ship : IShip
{
    public class Factory : PlaceholderFactory<ISlot, ISlot, ISlot, ISlot, IShipView, Ship>
    {
    }

    private readonly Subject<CardType> boarding = new Subject<CardType>();
    private readonly Subject<CardType> revealing = new Subject<CardType>();
    private readonly Subject<CardType> stashing = new Subject<CardType>();
    private readonly Subject<CardType> hacking = new Subject<CardType>();
    private readonly Subject<CardType> handling = new Subject<CardType>();
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    
    private readonly ISlot[] slots;
    private readonly IShipView view;
    private readonly IGameStatus gameStatus;

    private Ship(
        ISlot helm,
        ISlot plank, 
        ISlot storage,
        ISlot mount,
        IShipView view,
        IGameStatus gameStatus
    )
    {
        slots = new[] {plank, helm, storage, mount};

        Helm = helm;
        Plank = plank;
        Storage = storage;
        Mount = mount;

        this.view = view;
        this.gameStatus = gameStatus;
    }
    
    public ISlot Helm { get; }
    public ISlot Plank { get; }
    public ISlot Storage { get; }
    public ISlot Mount { get; }

    public IObservable<Unit> WhenArmed => Plank.WhenLodged
        .Where(card => card.IsRangeWeapon && card.IsStashed)
        .AsUnitObservable();

    public IObservable<CardType> WhenCardBoarded => boarding;
    public IObservable<CardType> WhenCardRevealed => revealing;
    public IObservable<CardType> WhenCardStashed => stashing;
    public IObservable<CardType> WhenCardHacked => hacking;
    public IObservable<CardType> WhenCardHandled => handling;

    public void Initialize()
    {
        disposables.Add(Plank.WhenLodged
            .Where(card => !card.IsBoarded)
            .Do(card =>
            {
                card.IsBoarded = true;
                
                boarding.OnNext(card.AbstractType);
            })
            .SelectMany(card => card.IsLocked 
                ? card.WhenUnlocked
                    .Do(_ => hacking.OnNext(card.AbstractType))
                    .ContinueWith(_ => card.DropAsObservable()
                        .Delay(TimeSpan.FromSeconds(0.25))
                        .Concat(Handle(card)))
                : Handle(card))
            .Subscribe());
    }

    public IObservable<Unit> ExpressBoard(ICard card)
    {
        return Observable.Create<Unit>(observer =>
        {
            if (card.IsResource)
            {
                card.IsBoarded = true;
                
                return Reveal(card)
                    .Merge(Stash(card))
                    .Subscribe(observer);
            }
            
            observer.OnError(new Exception($"Couldn't express-board '{card.Type}'"));

            return Disposable.Empty;
        });
    }

    public void Lock()
    {
        foreach (var slot in slots)
            slot.Lock();
    }

    public void Unlock()
    {
        foreach (var slot in slots)
        {
            if ((slot.Type & SlotType.Player) != 0)
                continue;

            slot.Unlock();
        }
    }

    private IObservable<Unit> Handle(ICard card)
    {
        return Reveal(card)
            .Delay(TimeSpan.FromSeconds(0.25))
            .ContinueWith(_ => card.IsResource
                ? Stash(card)
                : Observable.Empty<Unit>())
            .LastOrDefault()
            .Do(_ => handling.OnNext(card.Type));
    }

    private IObservable<Unit> Reveal(ICard card)
    {
        return card.Reveal()
            .DoOnSubscribe(() => revealing.OnNext(card.AbstractType));
    }
    
    private IObservable<Unit> Stash(ICard card)
    {
        return Observable.Create<Unit>(observer =>
        {
            var storage = card.IsItem ? Storage : (card.IsTool ? Mount : null);

            if (storage != null)
            {
                card.IsStashed = true;
                
                gameStatus.PlayerDidStashItem |= card.IsItem;
                gameStatus.PlayerDidStashTool |= card.IsTool;

                stashing.OnNext(card.Type);

                return storage.Lodge(card)
                    .Subscribe(observer);
            }

            observer.OnError(new Exception($"Unable to store Card '{card.Type}'"));

            return Disposable.Empty;
        });
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}
