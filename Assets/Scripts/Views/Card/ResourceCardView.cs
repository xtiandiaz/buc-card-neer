using UnityEngine;

public interface IResourceCardView : ICardView
{
    Sprite Item { set; }
    ISuit Suit { set; }
    int LockValue { set; }

    void ToggleLock(bool asEnabled);
}

public class ResourceCardView : CardView, IResourceCardView
{
    [SerializeField] private SpriteRenderer suitRenderer;
    [SerializeField] private SpriteRenderer itemRenderer;
    [SerializeField] private CardLabel lockLabel;

    public Sprite Item
    {
        set => itemRenderer.sprite = value;
    }

    public ISuit Suit
    {
        set
        {
            /*suitRenderer.sprite = value.Icon;
            suitRenderer.color = value.Color;*/
            valueLabel.SetColor(value.Color);
        }
    }

    public int LockValue
    {
        set => lockLabel.SetValue(value);
    }

    public void ToggleLock(bool asEnabled)
    {
        lockLabel.gameObject.SetActive(asEnabled);
    }
}