using System;
using UniRx;
using Zenject;

public enum CardType
{
    Player,
    Health, 
    Stamina,
    Defense,
    Ability,
    Baddie
}

public interface ICard
{
    CardType Type { get; }
    IObservable<bool> IsSelected { get; }
    IObservable<Coordinates> ObservableCoordinates { get; }
}

public abstract class Card : ICard
{
    private readonly ReactiveProperty<bool> isSelected = new ReactiveProperty<bool>();
    private readonly ReactiveProperty<Coordinates?> coordinates = new ReactiveProperty<Coordinates?>();

    protected Card(CardType type)
    {
        Type = type;
    }
    
    public CardType Type { get; }

    public Coordinates? Coordinates
    {
        get => coordinates.Value;
        set => coordinates.Value = value;
    }

    public IObservable<Coordinates> ObservableCoordinates => coordinates.Where(c => c.HasValue).Select(c => c.Value);
    public IObservable<bool> IsSelected => isSelected;

    public void Select()
    {
        isSelected.Value = true;
    }

    public void Deselect()
    {
        isSelected.Value = false;
    }
}