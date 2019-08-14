using TMPro;
using UnityEngine;

public interface IMerchantCardView : ICardView
{
    int Multiplier { set; }
}

public class MerchantCardView : CardView, IMerchantCardView
{
    [SerializeField] private TextMeshPro multiplierLabel = default;
    
    public override ISuitModel Suit
    {
        set
        {
            base.Suit = value;
            
            cardValue.Color = Color.white;
            multiplierLabel.color = value.Color;
        }
    }

    public int Multiplier
    {
        set => multiplierLabel.text = $"{value}×";
    }
}