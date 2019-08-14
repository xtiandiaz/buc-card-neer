using UnityEngine;

public interface IPlayerCardView : ICardView
{
    int Coins { set; }
    
    Vector3 HeartPosition { get; }
    Vector3 PouchPosition { get; }
}

public class PlayerCardView : CardView, IPlayerCardView
{
    [SerializeField] private CardValue coins = default;

    public int Coins
    {
        set => coins.SetValue(value);
    }

    public Vector3 HeartPosition => cardValue.transform.position;
    public Vector3 PouchPosition => coins.transform.position;
}