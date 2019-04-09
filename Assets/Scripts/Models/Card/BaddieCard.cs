using System;
using UniRx;
using Zenject;

public interface IBaddieCard
{
    int Stamina { get; }
    int Attack { get; }
    
    IObservable<int> ObservableStamina { get; }
    IObservable<int> ObservableAttack { get; }
}

public class BaddieCard : Card, IBaddieCard
{
    public class Factory : PlaceholderFactory<BaddieCard>
    {
    }
    
    private readonly ReactiveProperty<int> stamina;
    private readonly ReactiveProperty<int> attack;
    
    protected BaddieCard() : base(CardType.Baddie)
    {
        stamina = new ReactiveProperty<int>(UnityEngine.Random.Range(3, 11));
        attack = new ReactiveProperty<int>(UnityEngine.Random.Range(3, 11));
    }

    public int Stamina => stamina.Value;
    public int Attack => attack.Value;

    public IObservable<int> ObservableStamina => stamina;
    public IObservable<int> ObservableAttack => attack;
}