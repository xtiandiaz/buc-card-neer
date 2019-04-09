using System;
using System.Collections.Generic;
using UniRx;
using Zenject;

public interface ICardTile
{
    Coordinates Coordinates { get; }
    IObservable<bool> IsSelectedAsObservable { get; }
    IObservable<bool> IsLockedAsObservable { get; }
}


public class CardTile : ICardTile
{
    public class Factory : PlaceholderFactory<Coordinates, CardTile>
    {
    }
    
    private readonly ReactiveProperty<bool> isLocked = new ReactiveProperty<bool>();
    private readonly ReactiveProperty<bool> isSelected = new ReactiveProperty<bool>();
    private Card card;

    private CardTile(Coordinates coordinates)
    {
        Coordinates = coordinates;
        IsCenter = coordinates.x == 0 && coordinates.y == 0;
    }
    
    public Coordinates Coordinates { get; }
    public Dictionary<Direction, CardTile> Neighbors { get; } = new Dictionary<Direction, CardTile>();
    public bool IsLocked => isLocked.Value;
    public bool IsEdge { get; set; }
    public bool IsCenter { get; }
    
    public IObservable<bool> IsSelectedAsObservable => isSelected;
    public IObservable<bool> IsLockedAsObservable => isLocked;

    public Card Card
    {
        get => card;
        set
        {
            card = value;

            if (card != null)
                card.Coordinates = Coordinates;
        }
    }

    public void Select()
    {
        isSelected.Value = true;
    }

    public void Deselect()
    {
        isSelected.Value = false;
    }

    public void Lock()
    {
        isLocked.Value = true;
    }
    
    public void Unlock()
    {
        isLocked.Value = false;
    }
}