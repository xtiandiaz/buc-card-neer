using System;
using UniRx;
using UnityEngine;

public interface ICardPlayer : ICard, IPlayerStats
{
    bool CanPayFor(IResourceCard resourceCard);
    bool CanConsume(IResourceCard resourceCard);
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

    public override bool CanMatch(ICard withOther)
    {
        if ((withOther.Type & (CardType.Pirate)) != 0)
            return true;

        if (withOther is IResourceCard resourceCard)
            return CanPayFor(resourceCard) || CanConsume(resourceCard);

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

        if (!(withOther is IResourceCard resourceCard))
            return;

        if (CanPayFor(resourceCard))
            resourceCard.Purchase();
        else if (CanConsume(resourceCard))
            resourceCard.Consume();
    }

    public bool CanPayFor(IResourceCard resourceCard)
    {
        return !resourceCard.IsPurchase && resourceCard.Value <= Coins;
    }

    public bool CanConsume(IResourceCard resourceCard)
    {
        return resourceCard.IsPurchase & (resourceCard.Type & CardType.Food) != 0;
    }
}