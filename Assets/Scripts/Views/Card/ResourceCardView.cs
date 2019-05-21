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
    [SerializeField] private SpriteRenderer suitRenderer;
    [SerializeField] private CardLabel lockLabel;

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
            suitRenderer.sprite = value.Icon;
            suitRenderer.color = 
                containerRenderer.color =
                    lockLabel.Color = 
                        valueLabel.Color = value.Color;

            if ((value.ResourceType & ResourceType.Implement) == 0)
                return;
            
            frontFace.Color = value.Color;
            itemRenderer.color =
                valueLabel.Color = Color.white;
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