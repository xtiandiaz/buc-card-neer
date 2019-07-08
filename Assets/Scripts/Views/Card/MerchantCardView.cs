using UnityEngine;

public interface IMerchantCardView : ICardView
{
}

public class MerchantCardView : CardView, IMerchantCardView
{
    [SerializeField] private TextMesh multiplierLabel = default;

    public override int Value
    {
        set
        {
            base.Value = value;
            multiplierLabel.text = $"×{value}";
        }
    }

    public override ISuitModel Suit
    {
        set
        {
            base.Suit = value;
            
            multiplierLabel.color = value.Color;
            cardValue.Color = Color.white;
        }
    }
}