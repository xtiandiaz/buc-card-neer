using System;
using UniRx;

public interface ICardClasher
{
    IObservable<Unit> Clash(ISlot source, ISlot withDestination, Direction toward);
}

public class CardClasher : ICardClasher
{
    private readonly IAudioManager audioManager;

    private CardClasher(
        IAudioManager audioManager
        )
    {
        this.audioManager = audioManager;
    }
    
    public IObservable<Unit> Clash(ISlot source, ISlot withDestination, Direction toward)
    {
        return Observable.Create<Unit>(observer =>
        {
            if (CanClash(source, withDestination)) 
            { 
                var sourceCard = source.Peek();
                var destinationCard = withDestination.Peek();
                
                return sourceCard.Clash(destinationCard, toward)
                    .DoOnSubscribe(() => audioManager.Play(AudioEventSwitchKey.CardClash, sourceCard.Type))
                    .Subscribe(observer);
            }
            
            observer.OnCompleted();
                    
            return Disposable.Empty;
        });
    }
    
    private bool CanClash(ISlot fromSource, ISlot withDestination)
    {
        if (fromSource == null || fromSource.IsMessy || withDestination == null || withDestination.IsMessy)
            return false;

        return CanClash(fromSource.Peek(), withDestination.Peek());
    }
    
    private bool CanClash(ICard source, ICard withDestination)
    {
        if (source == null || withDestination == null)
            return false;

        switch (source.Type)
        {
            case CardType.Pirate:
                    
                return (withDestination.Type & CardType.Merchant) != 0;
                
            case CardType.Merchant:
                    
                return (withDestination.Type & CardType.Inspector) != 0;
                
            case CardType.Inspector:
                    
                return (withDestination.Type & CardType.Pirate) != 0;
                
            default:
                return false;
        }
    }
}