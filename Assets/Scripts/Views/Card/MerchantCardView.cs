using UnityEngine;

public interface IMerchantCardView : ICardView
{
    ISuit Suit { set; }
}

public class MerchantCardView : CardView, IMerchantCardView
{
    [SerializeField] private CardGraphic suitGraphic;

    public ISuit Suit
    {
        set
        {
            suitGraphic.Sprite = value.Icon;
            suitGraphic.Color = value.Color;
        }
    }
}