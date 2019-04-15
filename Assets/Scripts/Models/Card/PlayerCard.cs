using System;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public interface IPlayerCard
{
    IObservable<int> ObservableHealth { get; }
    IObservable<int> ObservableStamina { get; }
    IObservable<int> ObservableDefense { get; }
    IObservable<Tuple<AbilityType, int>> AcquiredAbility { get; }
}

public class PlayerCard : Card, IPlayerCard
{
    public class Factory : PlaceholderFactory<PlayerCard>
    {
    }
    
    private readonly GameSettings gameSettings;
    private readonly ReactiveProperty<int> health;
    private readonly ReactiveProperty<int> defense;
    private readonly ReactiveProperty<int> stamina;
    private readonly Subject<Tuple<AbilityType, int>> acquiredAbility = new Subject<Tuple<AbilityType, int>>();
    
    protected PlayerCard(GameSettings gameSettings) : base(CardType.Player)
    {
        this.gameSettings = gameSettings;
        
        health = new ReactiveProperty<int>(gameSettings.StartPlayerHealth);
        defense = new ReactiveProperty<int>(gameSettings.StartPlayerDefense);
        stamina = new ReactiveProperty<int>(gameSettings.StartPlayerStamina);
    }

    public int Health
    {
        get => health.Value;
        set => health.Value = Mathf.Clamp(value, 0, gameSettings.MaxPlayerHealth);
    }
    
    public int Stamina
    {
        get => stamina.Value;
        set => stamina.Value = Mathf.Clamp(value, 0, gameSettings.MaxPlayerStamina);
    }
    
    public int Defense
    {
        get => defense.Value;
        set => defense.Value = Mathf.Clamp(value, 0, gameSettings.MaxPlayerDefense);
    }
    
    public IObservable<int> ObservableHealth => health;
    public IObservable<int> ObservableDefense => defense;
    public IObservable<int> ObservableStamina => stamina;
    public IObservable<Tuple<AbilityType, int>> AcquiredAbility => acquiredAbility;

    public void Collect(ItemCard fromCard)
    {
        switch (fromCard.Type)
        {
            case CardType.Health:
                Health += fromCard.Value;
                break;
            case CardType.Stamina:
                Stamina += fromCard.Value;
                break;
            case CardType.Defense:
                Defense += fromCard.Value;
                break;
        }
    }

    public void Acquire(AbilityCard card)
    {
        acquiredAbility.OnNext(Tuple.Create(card.AbilityType, card.Index));
    }

    public void Perform(BaddieCard onCard)
    {
        var attackRemanent = Mathf.Max(0,onCard.Attack - Defense);
        
        Defense -= onCard.Attack;
        Health -= attackRemanent;
        Stamina -= Random.Range(1, onCard.Stamina + 1);
    }
}