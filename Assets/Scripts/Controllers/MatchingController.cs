using System;
using UniRx;

public interface IMatchingController : IDisposable
{
    IObservable<Unit> WhenMatched { get; }
    IObservable<DeviceType> WhenMatchedDevice { get; }
    IObservable<DeviceType> WhenDeviceActed { get; }
    
    bool CanMatch(ISlot fromSource, ISlot intoDestination);
    IObservable<Unit> Match(ISlot fromSource, ISlot intoDestination);
}

public class MatchingController : IMatchingController
{
    private readonly Subject<Unit> matching = new Subject<Unit>();
    private readonly Subject<Unit> attacking = new Subject<Unit>();
    private readonly Subject<Unit> confronting = new Subject<Unit>();
    private readonly Subject<DeviceType> deviceMatching = new Subject<DeviceType>();
    private readonly Subject<DeviceType> deviceActing = new Subject<DeviceType>();
    private readonly IPlayerCard player;
    private readonly IAudioManager audioManager;
    private readonly IShip ship;

    private MatchingController(
        IPlayerCard player,
        IAudioManager audioManager,
        IGameStatus gameStatus,
        IShip ship
        )
    {
        this.player = player;
        this.audioManager = audioManager;
        this.ship = ship;

        gameStatus.WhenPlayerAttackedOnBoard = attacking;
        gameStatus.WhenPlayerConfronted = confronting;
    }

    public IObservable<Unit> WhenMatched => matching;
    public IObservable<DeviceType> WhenDeviceActed => deviceActing;
    public IObservable<DeviceType> WhenMatchedDevice => deviceMatching;

    public bool CanMatch(ISlot fromSource, ISlot intoDestination)
    {
        switch (intoDestination.Type)
        {
            case SlotType.Player: 
            case SlotType.Boarding:
                
                return CanMatch(fromSource.Peek(), intoDestination.Peek());
            
            default:
                return false;
        }
    }

    public IObservable<Unit> Match(ISlot fromSource, ISlot intoDestination)
    {
        return Match(fromSource.Peek(), intoDestination.Peek())
            .Merge(
                fromSource.ConditionallyArrange(),
                intoDestination.ConditionallyArrange())
            .LastOrDefault();
    }

    public void Dispose()
    {
        matching.Dispose();
        attacking.Dispose();
        confronting.Dispose();
        deviceActing.Dispose();
        deviceMatching.Dispose();;
    }
    
    private bool CanMatch(ICard source, ICard withDestination)
    {
        if (source == null || !source.IsBoarded)
            return false;

        if (withDestination == null || !withDestination.IsBoarded)
            return false;

        if (source.IsDevice)
            return CanMatch((IDeviceCard) source, withDestination);

        switch (withDestination.Type)
        {
            case CardType.Player:

                return (source.Type & (CardType.Pirate)) != 0 ||
                       source.IsResource && 
                       (source.IsLocked || source.IsMedicine);

            case CardType.Pirate:

                return source.IsMeleeWeapon;
                
            case CardType.Merchant:

                return source.IsResource && source.IsStashed;

            case CardType.Food:
            case CardType.Artifact:
            case CardType.Gem:

                return withDestination.IsLocked && source.IsMeleeWeapon;
            
            default:
                return false;
        }
    }

    private bool CanMatch(IDeviceCard source, ICard withDestination)
    {
        switch (source.DeviceType)
        {
            case DeviceType.MidasTouch:
                return withDestination != player;
            case DeviceType.TraderSpell:
                return withDestination.IsMerchant;
            default:
                return false;
        }
    }
    
    private IObservable<Unit> Match(ICard source, ICard withDestination)
    {
        return Observable.Create<Unit>(observer =>
        {
            if (!CanMatch(source, withDestination))
            {
                observer.OnCompleted();
                
                return Disposable.Empty;
            }

            if (source.IsDevice)
            {
                var device = (IDeviceCard) source;
                
                return Match(device, withDestination)
                    .DoOnSubscribe(() => deviceMatching.OnNext(device.DeviceType))
                    .DoOnCompleted(() => deviceActing.OnNext(device.DeviceType))
                    .Subscribe(observer);
            }
            
            switch (withDestination.Type)
            {
                case CardType.Player:

                    switch (source.Type)
                    {
                        case CardType.Pirate:

                            var pirateHealth = source.Value;

                            return source.Hit(Math.Min(player.Value, pirateHealth))
                                .DoOnSubscribe(() =>
                                {
                                    audioManager.Play(AudioEventKey.CardConfrontPirate);
                                    confronting.OnNext(Unit.Default);
                                })
                                .Merge(player.Hit(pirateHealth)
                                    .Do(_ =>
                                    {
                                        if (player.HealthPoints > 0)
                                            player.Credit(source.OriginalValue);
                                    }))
                                .LastOrDefault()
                                .Subscribe(observer);

                        case CardType.Food:
                        case CardType.Artifact:
                        case CardType.Gem:

                            if (!source.IsLocked)
                                break;

                            var lockValue = source.LockValue;
                            
                            source.Hack(Math.Min(player.Value, lockValue));

                            return player.Hit(lockValue)
                                .DoOnSubscribe(() =>
                                {
                                    audioManager.Play(AudioEventKey.CardConfrontMonster);
                                    confronting.OnNext(Unit.Default);
                                })
                                .Subscribe(observer);
                        
                        case CardType.Medicine:
                            
                            player.Heal(source.Value);

                            return source.Hit(source.Value)
                                .DoOnSubscribe(() => audioManager.Play(AudioEventKey.CardToolHealthUse))
                                .Subscribe(observer);
                    }

                    break;
                case CardType.Pirate:
                    
                    return withDestination.Hit(source.Value)
                        .DoOnSubscribe(() =>
                        {
                            attacking.OnNext(Unit.Default);
                            audioManager.Play(AudioEventKey.CardToolMeleeUse);
                        })
                        .Do(_ =>
                        {
                            if (withDestination.Value > 0) 
                                return;
                            
                            player.Credit(withDestination.OriginalValue);
                            audioManager.Play(AudioEventKey.CardDefeatPirate);
                        })
                        .Merge(source.Destroy())
                        .LastOrDefault()
                        .Subscribe(observer);

                case CardType.Merchant:
                    
                    player.Credit(source.Value * 
                                  ((source.SuitType & withDestination.SuitType) != 0 
                                      ? MerchantCard.CreditMultiplierForMatchingSuit 
                                      : 1));
                    
                    audioManager.Play(AudioEventKey.CardItemTradeSell);

                    return source.Destroy()
                        .Subscribe(observer);
                
                case CardType.Food:
                case CardType.Artifact:
                case CardType.Gem:

                    withDestination.Hack(source.Value);

                   return source.Hit(source.Value)
                       .DoOnSubscribe(() => 
                       {
                           attacking.OnNext(Unit.Default);
                           audioManager.Play(AudioEventKey.CardToolMeleeUse);
                       })
                       .DoOnCompleted(() =>
                       {
                           if (withDestination.LockValue <= 0)
                               audioManager.Play(AudioEventKey.CardDefeatMonster);
                       })
                       .Subscribe(observer);

                default:

                    observer.OnError(new ArgumentOutOfRangeException());
                    observer.OnCompleted();
                    
                    break;
            }
            
            return Disposable.Empty;
        });
    }

    private IObservable<Unit> Match(IDeviceCard source, ICard withDestination)
    {
        return Observable.Create<Unit>(observer =>
        {
            switch (source.DeviceType)
            {
                case DeviceType.MidasTouch:
                    
                    player.Credit(withDestination.Value + player.Value);

                    return source.Destroy()
                        .Merge(withDestination.Destroy())
                        .Subscribe(observer);
                
                case DeviceType.TraderSpell:

                    var desiredSuit = ship.Storage.Peek()?.Suit;

                    return source.Destroy()
                        .DoOnSubscribe(() => ((IMerchantCard) withDestination).Resuit(desiredSuit))
                        .Subscribe(observer);
                
                default:

                    observer.OnError(
                        new Exception($"[MatchingController] Couldn't match with Device '{source.DeviceType}'"));
                    
                    return Disposable.Empty;
            }
        });
    }
}