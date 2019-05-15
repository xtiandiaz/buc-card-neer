using System;
using UniRx;
using UnityEngine;
using Zenject;

public interface ICardPlayer : ICard, IPlayerStats
{
    bool CanPurchase(ICardResource resourceCard);
    bool CanConsume(ICardResource resourceCard);
}

[CreateAssetMenu(fileName = "CardPlayer", menuName = "Game/Card/Player", order = 1)]
public class CardPlayer : Card, ICardPlayer, IPlayerStats
{
    [SerializeField] private IntReactiveProperty coins = new IntReactiveProperty(10);
    
    public override CardType Type => CardType.Player;

    public int HealthPoints
    {
        get => Value;
        set => Value = value;
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
        if ((fromSlot.Type & (SlotType.Storage | SlotType.Boarding)) == 0)
            return false;
        
        if ((withOther.Type & (CardType.Pirate)) != 0)
            return true;

        if (withOther is ICardResource resourceCard)
            return CanPurchase(resourceCard) || CanConsume(resourceCard);

        return false;
    }

    public override void Match(ICard withOther)
    {
        if ((withOther.Type & (CardType.Pirate)) != 0)
        {
            HealthPoints -= withOther.Value;
            withOther.Value = 0; // Cause for destruction
            
            return;
        }

        if (!(withOther is ICardResource resourceCard))
            return;

        if (CanPurchase(resourceCard))
            resourceCard.Purchase();
        else if (CanConsume(resourceCard))
            resourceCard.Consume();
    }

    public bool CanPurchase(ICardResource resourceCard)
    {
        return resourceCard.IsPurchasable && resourceCard.Value <= Coins;
    }

    public bool CanConsume(ICardResource resourceCard)
    {
        return resourceCard.IsConsumable;
    }
}