using System;
using UniRx;

public interface IClashingController
{
    IObservable<Unit> Clash(ISlot source, ISlot withDestination, Direction toward);
}

public class ClashingController : IClashingController
{
    private readonly IAudioManager audioManager;

    private ClashingController(
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
        if (fromSource == null || withDestination == null)
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

                return withDestination.IsMerchant;
                
            case CardType.Merchant:
                    
                return withDestination.IsMonster;

            default:
                return source.IsMonster && withDestination.IsPirate;
        }
    }
}