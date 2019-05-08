using System;
using UniRx;
using UnityEngine;

public interface ICardPlayer : ICard
{
    bool CanPayFor(IResourceCard resourceCard);
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
        set => coins.Value = value;
    }

    public IObservable<int> Health => value;
    public IObservable<int> Funds => coins;

    public override bool CanMatch(ICard withOther)
    {
        if ((withOther.Type & (CardType.Pirate)) != 0)
            return true;

        if (withOther is IResourceCard resourceCard)
            return !resourceCard.WasPaidFor && CanPayFor(resourceCard);

        return false;
    }

    public override void Match(ICard withOther)
    {
        if ((withOther.Type & (CardType.Pirate)) != 0)
        {
            HealthPoints -= withOther.Value;
            withOther.Destroy();
            
            return;
        }
        
        if (withOther is IResourceCard resourceCard && !resourceCard.WasPaidFor && CanPayFor(resourceCard))
            resourceCard.Purchase();
    }

    public bool CanPayFor(IResourceCard resourceCard)
    {
        return resourceCard.Value <= Coins;
    }
}