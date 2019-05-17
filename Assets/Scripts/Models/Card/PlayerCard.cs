using System;
using UniRx;
using UnityEngine;
using Zenject;

public interface IPlayerCard : ICard, IPlayerStats
{
    bool CanPurchase(IResourceCard resourceCard);
    bool CanConsume(IResourceCard resourceCard);
    bool CanUnlock(IResourceCard resourceCard);
}

[CreateAssetMenu(menuName = "Game/Card/Player")]
public class PlayerCard : Card, IPlayerCard
{
    [SerializeField] private int maxHealthPoints = 14;
    [SerializeField] private IntReactiveProperty coins = new IntReactiveProperty(10);
    
    public override CardType Type => CardType.Player;

    public int HealthPoints
    {
        get => Value;
        set => Value = Math.Min(value, maxHealthPoints);
    }

    public int Coins
    {
        get => coins.Value;
        set => coins.Value =  Mathf.Max(value, 0);
    }

    public IObservable<int> Health => value;
    public IObservable<int> Funds => coins;

    public override bool CanMatch(ICard withOther, ISlot fromSlot)
    {
        if (!withOther.IsBoarded)
            return false;
        
        if ((withOther.Type & (CardType.Pirate)) != 0)
            return true;

        if (withOther is IResourceCard resourceCard)
            return CanPurchase(resourceCard) || CanConsume(resourceCard) || CanUnlock(resourceCard);

        return false;
    }

    public override void Match(ICard withOther)
    {
        if (withOther is IPirateCard pirateCard)
        {
            HealthPoints -= pirateCard.Value;
            Coins += pirateCard.OriginalValue * pirateCard.LootMultiplier;
            
            pirateCard.Value = 0; // Cause for destruction
            
            return;
        }

        if (!(withOther is IResourceCard resourceCard))
            return;

        if (CanUnlock(resourceCard))
            resourceCard.Unlock();
        if (CanPurchase(resourceCard))
            resourceCard.Purchase();
        else if (CanConsume(resourceCard))
            resourceCard.Consume();
    }

    public bool CanUnlock(IResourceCard resourceCard)
    {
        return resourceCard.IsLocked && resourceCard.LockValue <= HealthPoints;
    }

    public bool CanPurchase(IResourceCard resourceCard)
    {
        return !resourceCard.IsAcquired && resourceCard.Value <= Coins;
    }

    public bool CanConsume(IResourceCard resourceCard)
    {
        return resourceCard.IsConsumable;
    }
}