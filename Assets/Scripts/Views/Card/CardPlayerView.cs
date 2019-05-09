using UnityEngine;

public interface ICardPlayerView : ICardView
{
    int CoinsValue { set; }
}

public class CardPlayerView : CardView, ICardPlayerView
{
    [SerializeField] private CardLabel coinsLabel;

    public int CoinsValue
    {
        set => coinsLabel.SetValue(value);
    }
}