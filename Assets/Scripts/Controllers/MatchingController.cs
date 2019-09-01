using System;
using UniRx;

public interface IMatchingController : IDisposable
{
    IObservable<Unit> WhenMatched { get; }
    
    bool CanMatch(ISlot fromSource, ISlot intoDestination);
    IObservable<Unit> Match(ISlot fromSource, ISlot intoDestination);
}

public class MatchingController : IMatchingController
{
    private readonly Subject<Unit> matching = new Subject<Unit>();
    private readonly Subject<Unit> attacking = new Subject<Unit>();
    private readonly IPlayerCard player;
    private readonly IArtificeMatchingController artificeMatcher;
    private readonly IConfrontationController confrontator;
    private readonly IAudioManager audioManager;
    private readonly IBoard board;

    private MatchingController(
        IArtificeMatchingController artificeMatcher,
        IConfrontationController confrontator,
        IPlayerCard player,
        IAudioManager audioManager,
        IGameStatus gameStatus,
        IBoard board
        )
    {
        this.artificeMatcher = artificeMatcher;
        this.confrontator = confrontator;
        this.audioManager = audioManager;
        this.player = player;
        this.board = board;

        gameStatus.WhenPlayerAttackedOnBoard = attacking;
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
                fromSource.ConditionallyArrange(),
                intoDestination.ConditionallyArrange())
            .LastOrDefault();
    }

    public void Dispose()
    {
        matching.Dispose();
        attacking.Dispose();
    }
    
    private bool CanMatch(ICard source, ICard withDestination)
    {
        if (source == null || !source.IsBoarded)
            return false;

        if (withDestination == null || !withDestination.IsBoarded)
            return false;

        if (source.IsArtifice)
            return artificeMatcher.CanMatch((IArtificeCard) source, withDestination);

        if (source.IsPlayer)
            return confrontator.CanConfront(withDestination);

        switch (withDestination.Type)
        {
            case CardType.Player:

                return source.IsMedicine || confrontator.CanConfront(source);

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
    
    private IObservable<Unit> Match(ICard source, ICard withDestination)
    {
        return Observable.Create<Unit>(observer =>
        {
            if (!CanMatch(source, withDestination))
            {
                observer.OnCompleted();
                
                return Disposable.Empty;
            }

            if (source.IsArtifice)
            {                
                return artificeMatcher.Match((IArtificeCard) source, withDestination)
                    .Subscribe(observer);
            }

            if (source.IsPlayer)
            {
                return confrontator.Confront(withDestination)
                    .Subscribe(observer);
            }
            
            switch (withDestination.Type)
            {
                case CardType.Player:

                    switch (source.Type)
                    {
                        case CardType.Pirate:
                        // MONSTER:
                        case CardType.Food:
                        case CardType.Artifact:
                        case CardType.Gem:

                            return confrontator.Confront(source)
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
}