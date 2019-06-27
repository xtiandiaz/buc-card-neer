using System;
using UniRx;
using UnityEngine;
using Zenject;

public interface IPlayerCard : ICard
{
    int HealthPoints { get; }
    int Coins { get; }

    void Heal(int byAmount);
    void Debit(int amount);
    void Credit(int amount);
}

public class PlayerCard : Card, IPlayerCard
{
    public class Factory : PlaceholderFactory<IPlayerCardModel, IPlayerCardView, PlayerCard>
    {
    }

    private readonly int maxHealthPoints;
    private readonly ReactiveProperty<int> coins;

    private PlayerCard(IPlayerCardModel model, IPlayerCardView view)
        : base(model, view)
    {
        IsBoarded = true;
        HealthPoints = maxHealthPoints = model.MaxHealthPoints;
        coins = new ReactiveProperty<int>(model.InitialCoins);

        view.Coins = model.InitialCoins;
    }

    public int Coins
    {
        get => coins.Value;
        private set => coins.Value = Mathf.Max(value, 0);
    }

    public int HealthPoints
    {
        get => Value;
        private set => Value = Math.Min(value, maxHealthPoints);
    }

    public void Heal(int byAmount)
    {
        HealthPoints += byAmount;
    }

    public void Debit(int amount)
    {
        Coins -= amount;
    }

    public void Credit(int amount)
    {
        Coins += amount;
    }
}