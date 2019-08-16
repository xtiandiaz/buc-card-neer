using System;
using UniRx;
using UnityEngine;

public interface ICardHost
{
    bool CanLodge(ISlot fromSource, ISlot intoDestination);
    IObservable<Unit> Lodge(ICard card, ISlot intoDestination);
    IObservable<Unit> Lodge(ISlot fromSource, ISlot intoDestination);
}

public class CardHost : ICardHost
{
    public bool CanLodge(ISlot fromSource, ISlot intoDestination)
    {
        var sourceCard = fromSource.Peek();

        if (sourceCard == null ||
            !intoDestination.HasRoom ||
            intoDestination.DoesContain(sourceCard))
            return false;
        
        switch (intoDestination.Type)
        {
            case SlotType.Boarding:

                if ((fromSource.Type & (SlotType.Supply | SlotType.Mount)) == 0)
                    return false;

                break;
            case SlotType.Storage:
            case SlotType.Mount:

                if ((fromSource.Type & SlotType.Boarding) == 0)
                    return false;

                break;
        }
        
        return CanLodge(sourceCard, intoDestination);
    }
    
    public IObservable<Unit> Lodge(ICard card, ISlot intoDestination)
    {
        return Observable.Create<Unit>(observer =>
        {
            if (!CanLodge(card, intoDestination))
            {
                observer.OnError(new Exception("Can't lodge Card"));

                return Disposable.Empty;
            }

            return intoDestination.Lodge(card)
                .Subscribe(observer);
            
        }).DoOnError(Debug.LogException);
    }

    public IObservable<Unit> Lodge(ISlot fromSource, ISlot intoDestination)
    {
        return Observable.Create<Unit>(observer =>
        {
            if (!CanLodge(fromSource, intoDestination))
            {
                observer.OnError(new Exception("Can't lodge Card"));

                return Disposable.Empty;
            }

            return intoDestination.Lodge(fromSource.Pop(), SlotLodgingMode.Voluntary, true)
                .Merge(fromSource.ConditionallyArrange())
                .AsSingleUnitObservable()
                .Subscribe(observer);
            
        }).DoOnError(Debug.LogException);
    }

    private bool CanLodge(ICard card, ISlot intoDestination)
    {
        switch (intoDestination.Type)
        {
            case SlotType.Boarding:
                
                if (!card.IsBoarded)
                    return (card.Type & (CardType.Resource | CardType.Agent)) != 0;

                return intoDestination.IsEmpty && (card.Type & (CardType.WeaponRanged | CardType.Device)) != 0;

            case SlotType.Player:

                return (card.Type & CardType.Player) != 0;

            case SlotType.Storage:

                return card.IsBoarded && !card.IsLocked && card.IsItem;
                
            case SlotType.Mount:

                return card.IsBoarded && !card.IsLocked && card.IsTool;

            default:
                return false;
        }
    }
}