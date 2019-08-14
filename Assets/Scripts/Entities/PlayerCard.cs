using System;
using UniRx;
using UnityEngine;
using Zenject;

public interface IPlayerCard : ICard
{
    int HealthPoints { get; }
    int Coins { get; }
    
    Vector3 HeartPosition { get; }
    Vector3 PouchPosition { get; }
    
    IObservable<int> WhenHealed { get; }
    IObservable<int> WhenDebited { get; }
    IObservable<int> WhenCredited { get; }

    void Heal(int byAmount);
    void Credit(int amount);
}

public class PlayerCard : Card, IPlayerCard
{
    public new class Factory : PlaceholderFactory<IPlayerCardModel, IPlayerCardView, PlayerCard>
    {
    }

    private readonly Subject<int> healing = new Subject<int>();
    private readonly Subject<int> crediting = new Subject<int>();
    private readonly Subject<int> debiting = new Subject<int>();
    private readonly ReactiveProperty<int> coins;
    
    private readonly int maxHealthPoints;
    private readonly IPlayerCardView view;

    private PlayerCard(IPlayerCardModel model, IPlayerCardView view)
        : base(model, view)
    {
        IsBoarded = true;
        HealthPoints = maxHealthPoints = model.MaxHealthPoints;
        coins = new ReactiveProperty<int>(model.InitialCoins);

        view.Coins = model.InitialCoins;
        this.view = view;
    }

    public int Coins
    {
        get => coins.Value;
        private set => coins.Value = view.Coins = Mathf.Max(value, 0);
    }

    public int HealthPoints
    {
        get => Value;
        private set => Value = Math.Min(value, maxHealthPoints);
    }

    public Vector3 HeartPosition => view.HeartPosition;
    public Vector3 PouchPosition => view.PouchPosition;

    public IObservable<int> WhenHealed => healing;
    public IObservable<int> WhenCredited => crediting;
    public IObservable<int> WhenDebited => debiting;

    public void Heal(int byAmount)
    {
        HealthPoints += byAmount;
        
        healing.OnNext(byAmount);
    }

    public void Debit(int amount)
    {
        Coins -= amount;
        
        debiting.OnNext(amount);
    }

    public void Credit(int amount)
    {
        Coins += amount;
        
        crediting.OnNext(amount);
    }

    public override void Dispose()
    {
        base.Dispose();
        
        coins.Dispose();
        healing.Dispose();
        crediting.Dispose();
        debiting.Dispose();
    }
}