using UnityEngine;

public class CardFoeView : CardView
{
    [SerializeField] private TextMesh valueText;
    
    public int Value
    {
        set => valueText.text = value.ToString();
    }
}