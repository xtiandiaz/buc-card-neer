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
    private readonly Subject<Unit> matching = new Subject<Unit>();
    private readonly IPlayerCard player;

    private CardMatcher(IPlayerCard player)
    {
        this.player = player;
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
            .Do(matching.OnNext)
            .LastOrDefault();
    }

    public void Dispose()
    {
        matching?.Dispose();
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
                                .Merge(player.Hit(pirateHealth))
                                .LastOrDefault()
                                .Subscribe(observer);
                        
                        case CardType.Inspector:

                            var inspectorBribe = source.Value;

                            return source.Hit(Math.Min(player.Coins, inspectorBribe))
                                .Merge(player.DebitAsObservable(inspectorBribe))
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
                                .IgnoreElements()
                                .Subscribe(observer);
                        
                        case CardType.Medicine:
                            
                            player.Heal(source.Value);

                            return source.Hit(source.Value)
                                .Subscribe(observer);
                    }

                    break;
                case CardType.Pirate:
                case CardType.Inspector:

                    return withDestination.Hit(source.Value)
                        .Merge(source.Destroy())
                        .LastOrDefault()
                        .Subscribe(observer);
                
                case CardType.Merchant:

                    // TODO Credit with multiplier
                    player.Credit(source.Value);

                    return source.Destroy()
                        .Subscribe(observer);
                
                case CardType.Food:
                case CardType.Artifact:
                case CardType.Gem:

                    withDestination.Hack(source.Value);

                    return source.Hit(source.Value)
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