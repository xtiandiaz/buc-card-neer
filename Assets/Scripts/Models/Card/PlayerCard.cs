using System;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class PlayerCard : Card
{
    public class Factory : PlaceholderFactory<PlayerCard>
    {
    }
    
    private readonly GameSettings gameSettings;
    private readonly ReactiveProperty<int> health;
    private readonly ReactiveProperty<int> defense;
    private readonly ReactiveProperty<int> stamina;
    
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

    public void Collect(ItemCard fromCard)
    {
    }

    public void Acquire(MerchantCard card)
    {
    }

    public void Perform(PirateCard onCard)
    {
    }
}