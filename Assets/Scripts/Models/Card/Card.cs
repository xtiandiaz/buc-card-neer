using System;
using UniRx;
using Zenject;

public enum CardType
{
    Player,
    Item, 
    Merchant,
    Pirate,
    Inspector
}

public interface ICard
{
    CardType Type { get; }
    IObservable<bool> IsSelected { get; }
}

public abstract class Card : ICard
{
    private readonly ReactiveProperty<bool> isSelected = new ReactiveProperty<bool>();

    protected Card(CardType type)
    {
        Type = type;
    }
    
    public CardType Type { get; }

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