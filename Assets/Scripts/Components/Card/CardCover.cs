using UnityEngine;

public interface ICardCover
{
    Sprite Cover { set; }
    
    void ToggleContent(bool toValue);
    void ToggleVisibility(bool toValue);
}

public class CardCover : MonoBehaviour, ICardCover
{
    [SerializeField] private SpriteRenderer coverRenderer;
    [SerializeField] private GameObject contentWrapper;

    public Sprite Cover
    {
        set => coverRenderer.sprite = value;
    }

    public Color Color
    {
        set => coverRenderer.color = value;
    }

    public void ToggleVisibility(bool toValue)
    {
        gameObject.SetActive(toValue);
    }

    public void ToggleContent(bool toValue)
    {
        if (contentWrapper != null)
            contentWrapper.SetActive(toValue);
    }
}