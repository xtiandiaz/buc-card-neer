using System;
using System.Linq;
using UniRx;

public interface ICardShooter : IDisposable
{
    IObservable<Unit> WhenHit { get; }
    
    bool CanShoot(ISlot fromSource, ISlot intoDestination);
    IObservable<Unit> Shoot(ISlot fromSource, ISlot intoDestination);
    IObservable<Unit> Shoot(ISlot fromSource, ISlot[] intoDestinations);
}

public class CardShooter : ICardShooter
{
    private readonly IAudioManager audioManager;
    private readonly Subject<Unit> shooting = new Subject<Unit>();
    private readonly Subject<Unit> hitting = new Subject<Unit>();

    private CardShooter(
        IAudioManager audioManager,
        IGameStatus gameStatus
    )
    {
        this.audioManager = audioManager;

        gameStatus.WhenPlayerShot = shooting;
    }
    
    public IObservable<Unit> WhenHit => hitting;
    
    public bool CanShoot(ISlot fromSource, ISlot intoDestination)
    { 
        return (fromSource.Type & SlotType.Boarding) != 0 &&
               CanShoot(fromSource.Peek(), intoDestination.Peek());
    }
    
    public IObservable<Unit> Shoot(ISlot fromSource, ISlot[] intoDestinations)
    {
        return Observable.Create<Unit>(observer =>
        {
            var weapon = fromSource.Peek();
            
            if (!CanShoot(weapon))
            {
                observer.OnCompleted();
                return Disposable.Empty;
            }
            
            var targets = intoDestinations.Select(slot => slot.Peek()).ToArray();

            audioManager.Play(AudioEventKey.CardToolRangedUseCannon);

            shooting.OnNext(Unit.Default);

            return Observable.Timer(TimeSpan.FromSeconds(0.25))
                .Do(_ => audioManager.Play(AudioEventKey.CardToolRangedHitCannon))
                .ContinueWith(Shoot(weapon, targets)
                    .Merge(weapon.Destroy()))
                .LastOrDefault()
                .Do(hitting.OnNext)
                .Subscribe(observer);
        });
    }

    public IObservable<Unit> Shoot(ISlot fromSource, ISlot intoDestination)
    {
        return Shoot(fromSource.Peek(), intoDestination.Peek())
            .Merge(fromSource.Pop()?.Destroy())
            .LastOrDefault();
    }

    private bool CanShoot(ICard withSource)
    {
        return withSource != null && withSource.IsRangeWeapon;
    }
    
    private bool CanShoot(ICard withSource, ICard toDestination)
    {
        return CanShoot(withSource) && 
               toDestination != null && toDestination.IsRangeTarget;
    }

    private IObservable<Unit> Shoot(ICard withSource, ICard[] atTargets)
    {
        return Observable.Range(0, atTargets.Length)
            .Select(i => Shoot(withSource, atTargets[i]))
            .Merge();
    }

    private IObservable<Unit> Shoot(ICard withSource, ICard atTarget)
    {
        return Observable.Create<Unit>(observer =>
        {
            if (CanShoot(withSource, atTarget))
            {
                return atTarget.OnShot(withSource.Value)
                    .Subscribe(observer);
            }
            
            observer.OnCompleted();
                
            return Disposable.Empty;
        });
    }

    public void Dispose()
    {
        shooting?.Dispose();
    }
}