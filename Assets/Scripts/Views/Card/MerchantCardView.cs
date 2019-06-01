using UnityEngine;

public interface IMerchantCardView : ICardView
{
    ISuit Suit { set; }
}

public class MerchantCardView : CardView, IMerchantCardView
{
    [SerializeField] private CardGraphic suitGraphic;
    [SerializeField] private TextMesh multiplierLabel;

    public override int Value
    {
        set
        {
            base.Value = value;
            multiplierLabel.text = $"×{value}";
        }
    }

    public ISuit Suit
    {
        set
        {
            suitGraphic.Sprite = value.Icon;
            suitGraphic.Color = value.Color;
            valueLabel.Color = 
                multiplierLabel.color = value.Color;
        }
    }
}