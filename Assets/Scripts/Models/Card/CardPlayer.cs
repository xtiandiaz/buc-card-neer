using System;
using UniRx;
using UnityEngine;

public interface ICardPlayer : ICard, IPlayerStats
{
}

[CreateAssetMenu(fileName = "CardPlayer", menuName = "Game/Card/Player", order = 1)]
public class CardPlayer : Card, ICardPlayer
{
    [SerializeField] private IntReactiveProperty coins = new IntReactiveProperty(10);
    
    public override CardType Type => CardType.Player;
    public override CardType InteractionMask => CardType.Resource | CardType.Pirate;

    public int HealthPoints
    {
        get => value.Value;
        set => this.value.Value = value;
    }

    public int Coins
    {
        get => coins.Value;
        set => coins.Value = value;
    }

    public IObservable<int> Health => value;
    public IObservable<int> Funds => coins;

    public override bool DoesConsume(ICard other)
    {
        if ((other.Type & CardType.Pirate) != 0)
        {
            HealthPoints -= other.Value;
            other.Value = 0;

            return true;
        }
        
        if (other is IResourceCard resourceCard && !resourceCard.WasPaidFor)
        {
            return resourceCard.Buy();
        }

        return false;
    }
}