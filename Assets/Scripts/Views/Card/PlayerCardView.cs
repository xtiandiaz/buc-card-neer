using UnityEngine;

public interface IPlayerCardView : ICardView
{
    int MaxHealth { set; }
    int Coins { set; }
    
    Vector3 HeartPosition { get; }
    Vector3 PouchPosition { get; }
}

public class PlayerCardView : CardView, IPlayerCardView
{
    [SerializeField] private CardValue coins = default;

    public int MaxHealth { private get; set; }
    
    public int Coins
    {
        set => coins.SetValue(value);
    }

    public override int Value
    {
        set => cardValue.SetValue($"{value}<size=3.5> / {MaxHealth}</size>"); 
    }

    public Vector3 HeartPosition => cardValue.transform.position;
    public Vector3 PouchPosition => coins.transform.position;
}