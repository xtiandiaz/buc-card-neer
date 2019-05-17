using UnityEngine;

public interface IPlayerCardView : ICardView
{
    int CoinsValue { set; }
}

public class PlayerCardView : CardView, IPlayerCardView
{
    [SerializeField] private CardLabel coinsLabel;

    public int CoinsValue
    {
        set => coinsLabel.SetValue(value);
    }
}