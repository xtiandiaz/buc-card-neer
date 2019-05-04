using UnityEngine;

public class CardResourceView : CardView
{
    [SerializeField] private SpriteRenderer suitRenderer;
    [SerializeField] private SpriteRenderer itemRenderer;

    public Sprite Item
    {
        set => itemRenderer.sprite = value;
    }

    public ISuit Suit
    {
        set
        {
            suitRenderer.sprite = value.Icon;
            suitRenderer.color = value.Color;
            cardValue.SetColor(value.Color);
        }
    }
}