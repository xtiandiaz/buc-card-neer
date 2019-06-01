using UnityEngine;

public interface IResourceCardView : ICardView
{
    Sprite Container { set; }
    Sprite Item { set; }
    ISuit Suit { set; }
    int LockValue { set; }

    void ToggleLock(bool asEnabled);
}

public class ResourceCardView : CardView, IResourceCardView
{
    [SerializeField] private SpriteRenderer containerRenderer;
    [SerializeField] private SpriteRenderer itemRenderer;
    [SerializeField] private CardGraphic suitGraphic;
    [SerializeField] private CardLabel lockLabel;
    [SerializeField] private SpriteRenderer borderRenderer;

    public Sprite Container
    {
        set => containerRenderer.sprite = value;
    }
    
    public Sprite Item
    {
        set => itemRenderer.sprite = value;
    }

    public ISuit Suit
    {
        set
        {
            suitGraphic.Sprite = value.Icon;
            itemRenderer.color = 
                valueLabel.Color = 
                    suitGraphic.Color = 
                        borderRenderer.color = value.Color;

            if ((value.ResourceType & ResourceType.Tool) != 0)
                return;
            
            backFace.Color = value.Color;
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