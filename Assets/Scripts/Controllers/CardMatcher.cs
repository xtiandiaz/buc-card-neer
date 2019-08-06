using System;
using UniRx;

public interface ICardMatcher : IDisposable
{
    IObservable<Unit> WhenMatched { get; }
    
    bool CanMatch(ISlot fromSource, ISlot intoDestination);
    IObservable<Unit> Match(ISlot fromSource, ISlot intoDestination);
}

public class CardMatcher : ICardMatcher
{
    private const int CreditMultiplierForMatchingSuit = 2;
    
    private readonly Subject<Unit> matching = new Subject<Unit>();
    private readonly Subject<Unit> attacking = new Subject<Unit>();
    private readonly Subject<Unit> confronting = new Subject<Unit>();
    private readonly IPlayerCard player;
    private readonly IAudioManager audioManager;

    private CardMatcher(
        IPlayerCard player,
        IAudioManager audioManager,
        IGameStatus gameStatus
        )
    {
        this.player = player;
        this.audioManager = audioManager;

        gameStatus.WhenPlayerAttackedOnBoard = attacking;
        gameStatus.WhenPlayerConfronted = confronting;
    }

    public IObservable<Unit> WhenMatched => matching;

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
                fromSource.ConditionallyArrange().IgnoreElements(),
                intoDestination.ConditionallyArrange().IgnoreElements())
            // Only bubble up matching elements by ignoring the arrangement ones
            .Do(matching.OnNext)
            // Order matters for some emissions are ignored within the matching stream
            .LastOrDefault();
    }

    public void Dispose()
    {
        matching.Dispose();
        attacking.Dispose();
        confronting.Dispose();
    }
    
    private bool CanMatch(ICard source, ICard withDestination)
    {
        if (source == null || !source.IsBoarded)
            return false;

        if (withDestination == null || !withDestination.IsBoarded)
            return false;

        switch (withDestination.Type)
        {
            case CardType.Player:

                return (source.Type & (CardType.Pirate | CardType.Inspector)) != 0 ||
                       source.IsResource && 
                       (source.IsLocked || source.IsMedicine);

            case CardType.Pirate:

                return source.IsMeleeWeapon;
                
            case CardType.Merchant:

                return source.IsResource && source.IsStored;
                
            case CardType.Inspector:
                
                return source.IsItem;
            
            case CardType.Food:
            case CardType.Artifact:
            case CardType.Gem:

                return withDestination.IsLocked && source.IsMeleeWeapon;
            
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
                        
                        case CardType.Inspector:

                            var inspectorBribe = source.Value;

                            return source.Hit(Math.Min(player.Coins, inspectorBribe))
                                .DoOnSubscribe(() => audioManager.Play(AudioEventKey.CardConfrontMarshal))
                                .Merge(player.Debit(inspectorBribe))
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
                                .IgnoreElements()
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
                    
                case CardType.Inspector:

                    return withDestination.Hit(source.Value)
                        .DoOnSubscribe(() => audioManager.Play(AudioEventKey.CardItemBribe))
                        .Do(_ =>
                        {
                            if (withDestination.Value <= 0)
                                audioManager.Play(AudioEventKey.CardDefeatMarshal);
                        })
                        .Merge(source.Destroy())
                        .LastOrDefault()
                        .Subscribe(observer);
                
                case CardType.Merchant:
                    
                    player.Credit(source.Value * 
                                  ((source.Suit & withDestination.Suit) != 0 ? CreditMultiplierForMatchingSuit : 1));
                    
                    audioManager.Play(AudioEventKey.CardItemTradeSell);

                    return source.Destroy()
                        .Subscribe(observer);
                
                case CardType.Food:
                case CardType.Artifact:
                case CardType.Gem:

                    withDestination.Hack(source.Value);

                    var result = source.Hit(source.Value)
                        .DoOnSubscribe(() =>
                        {
                            attacking.OnNext(Unit.Default);
                            audioManager.Play(AudioEventKey.CardToolMeleeUse);
                        })
                        .DoOnCompleted(() =>
                        {
                            if (withDestination.LockValue <= 0)
                                audioManager.Play(AudioEventKey.CardDefeatMonster);
                        });

                    if (withDestination.LockValue <= 0)
                        result = result.IgnoreElements();
                    
                    return result.Subscribe(observer);

                default:

                    observer.OnError(new ArgumentOutOfRangeException());
                    observer.OnCompleted();
                    
                    break;
            }
            
            return Disposable.Empty;
        });
    }
}