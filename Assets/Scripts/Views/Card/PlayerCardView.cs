using UnityEngine;

public interface IPlayerCardView : ICardView
{
    int Coins { set; }
}

public class PlayerCardView : CardView, IPlayerCardView
{
    [SerializeField] private CardValue coins;

    public int Coins
    {
        set => coins.SetValue(value);
    }
}