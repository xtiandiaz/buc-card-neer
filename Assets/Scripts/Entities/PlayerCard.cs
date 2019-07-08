using System;
using UniRx;
using UnityEngine;
using Zenject;

public interface IPlayerCard : ICard
{
    int HealthPoints { get; }
    int Coins { get; }
    
    IObservable<Unit> WhenBankrupt { get; }

    void Heal(int byAmount);
    void Debit(int amount);
    IObservable<Unit> DebitAsObservable(int amount);
    void Credit(int amount);
}

public class PlayerCard : Card, IPlayerCard
{
    public new class Factory : PlaceholderFactory<IPlayerCardModel, IPlayerCardView, PlayerCard>
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

    public IObservable<Unit> WhenBankrupt => coins.Where(value => value <= 0).AsSingleUnitObservable();

    public void Heal(int byAmount)
    {
        HealthPoints += byAmount;
    }

    public void Debit(int amount)
    {
        Coins -= amount;
    }

    public IObservable<Unit> DebitAsObservable(int amount)
    {
        return Observable.Create<Unit>(observer =>
        {
            Coins -= amount;
            
            if (Coins <= 0)
                return Destroy().Subscribe(observer);
            
            observer.OnNext(Unit.Default);
            observer.OnCompleted();
            
            return Disposable.Empty;
        });
    }

    public void Credit(int amount)
    {
        Coins += amount;
    }
}