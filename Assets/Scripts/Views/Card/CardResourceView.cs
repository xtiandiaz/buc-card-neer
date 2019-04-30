using UnityEngine;

public class CardResourceView : CardView
{
    [SerializeField] private TextMesh valueText;
    
    public int Value
    {
        set => valueText.text = value.ToString();
    }
}