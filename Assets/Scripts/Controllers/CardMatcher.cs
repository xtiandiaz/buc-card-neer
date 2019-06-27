using System;
using UniRx;

public interface ICardMatcher
{
    bool CanMatch(ISlot fromSource, ISlot intoDestination);
    IObservable<Unit> Match(ISlot fromSource, ISlot intoDestination);
}

public class CardMatcher : ICardMatcher
{
    private readonly IPlayerCard player;

    private CardMatcher(IPlayerCard player)
    {
        this.player = player;
    }

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
        return Observable.Create<Unit>(observer =>
        {
            return Match(fromSource.Peek(), intoDestination.Peek())
                .Do(_ => 
                {
                    if (fromSource.Peek()?.IsExhausted == true)
                        fromSource.Pop().Destroy();
                    
                    if (intoDestination.Peek()?.IsExhausted == true)
                        intoDestination.Pop().Destroy();
                })
                .Subscribe(observer);
        });
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
            if (CanMatch(source, withDestination))
            {
                switch (withDestination.Type)
                {
                    case CardType.Player:

                        switch (source.Type)
                        {
                            case CardType.Pirate:

                                var pirateHealth = source.Value;

                                source.Hit(Math.Min(player.Value, pirateHealth));

                                player.Hit(pirateHealth);

                                break;
                            case CardType.Inspector:

                                var inspectorBribe = source.Value;

                                source.Hit(Math.Min(player.Coins, inspectorBribe));

                                player.Debit(inspectorBribe);

                                break;
                            case CardType.Food:
                            case CardType.Artifact:
                            case CardType.Gem:

                                if (!source.IsLocked)
                                    break;

                                var lockValue = source.LockValue;

                                source.Hack(Math.Min(player.Value, lockValue));

                                player.Hit(lockValue);

                                break;
                            case CardType.Medicine:
                                
                                player.Heal(source.Value);

                                source.Hit(source.Value);

                                break;
                        }

                        break;
                    case CardType.Pirate:

                        withDestination.Hit(source.Value);

                        source.IsExhausted = true;

                        break;
                    case CardType.Merchant:

                        // TODO Credit with multiplier
                        player.Credit(source.Value);

                        source.IsExhausted = true;

                        break;
                    case CardType.Inspector:

                        withDestination.Hit(source.Value);

                        source.IsExhausted = true;

                        break;
                    case CardType.Food:
                    case CardType.Artifact:
                    case CardType.Gem:

                        withDestination.Hack(source.Value);

                        source.Hit(source.Value);

                        break;
                    default:

                        observer.OnError(new ArgumentOutOfRangeException());
                        
                        break;
                }
            }
            
            observer.OnNext(Unit.Default);
            observer.OnCompleted();

            return Disposable.Empty;
        });
    }
}