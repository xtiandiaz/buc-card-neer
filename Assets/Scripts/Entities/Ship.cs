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
    
    IObservable<Unit> ExpressHandle(ICard card);
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
                Lock();
                Board(card);
            })
            .SelectMany(card =>
            {
                if (card.IsResource)
                {
                    return card.IsLocked 
                        ? HandleLockedResource(card) 
                        : HandleResource(card);
                }

                return card.IsAgent 
                    ? HandleAgent(card) 
                    : card.Reveal();
            })
            .Subscribe());
    }

    public IObservable<Unit> ExpressHandle(ICard card)
    {
        return Observable.Create<Unit>(observer =>
        {
            if (card.IsResource)
            {
                Board(card);

                return Reveal(card)
                    .Merge(Stash(card))
                    .DoOnCompleted(() => handling.OnNext(card.Type))
                    .Subscribe(observer);
            }
            
            observer.OnError(new Exception($"Couldn't express-handle '{card.Type}'"));

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
    
    public void Dispose()
    {
        boarding.Dispose();
        revealing.Dispose();
        stashing.Dispose();
        handling.Dispose();
        hacking.Dispose();
        
        disposables.Dispose();
    }

    private void Board(ICard card)
    {
        card.IsBoarded = true;
        
        // The following check for whether the monster is locked or not is necessary for express-handling:
        boarding.OnNext(card.IsMonster && card.IsLocked ? CardType.Monster : card.Type);
    }

    private IObservable<Unit> HandleAgent(ICard card)
    {
        return Observable.Create<Unit>(observer =>
        {
            if (!card.IsAgent)
            {
                observer.OnError(new Exception("Can't handle non-Agent card"));
                return Disposable.Empty;
            }

            return card.Reveal()
                .DoOnCompleted(Unlock)
                .ContinueWith(card.WhenDestroyed)
                .DoOnCompleted(() => handling.OnNext(card.Type))
                .Subscribe(observer);
        });
    }

    private IObservable<Unit> HandleLockedResource(ICard card)
    {
        return Observable.Create<Unit>(observer =>
        {
            if (!card.IsResource || !card.IsLocked)
            {
                observer.OnError(new Exception("Can't handle non-locked resource"));
                return Disposable.Empty;
            }
            
            Unlock();

            return card.WhenUnlocked
                .Do(_ =>
                {
                    Lock();
                    hacking.OnNext(card.AbstractType);
                })
                .ContinueWith(_ => card.DropAsObservable()
                    .Delay(TimeSpan.FromSeconds(0.25))
                    .Concat(HandleResource(card)))
                .Subscribe(observer);
        });
    }

    private IObservable<Unit> HandleResource(ICard card)
    {
        return Observable.Create<Unit>(observer =>
        {
            if (!card.IsResource)
            {
                observer.OnError(new Exception("Can't handle non-Resource card."));
                return Disposable.Empty;
            }

            return Reveal(card)
                .Delay(TimeSpan.FromSeconds(0.25))
                .ContinueWith(_ => Stash(card))
                .DoOnCompleted(() =>
                {
                    Unlock();
                    handling.OnNext(card.Type);
                })
                .Subscribe(observer);
        });
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

                return storage.Lodge(card, SlotLodgingMode.Systematic, true)
                    .Subscribe(observer);
            }

            observer.OnError(new Exception($"Unable to store Card '{card.Type}'"));

            return Disposable.Empty;
        });
    }
}
