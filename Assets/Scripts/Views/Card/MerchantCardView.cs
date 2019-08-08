using UnityEngine;

public interface IMerchantCardView : ICardView
{
}

public class MerchantCardView : CardView, IMerchantCardView
{
    public override ISuitModel Suit
    {
        set
        {
            base.Suit = value;
            
            cardValue.Color = Color.white;
        }
    }
}