using UnityEngine;

public interface ICardFaceView
{
    Sprite Sprite { set; }
    void ToggleVisibility(bool on);
    void ToggleContent(bool on);
}

public class CardFaceView : MonoBehaviour, ICardFaceView
{
    [SerializeField] private SpriteRenderer faceRenderer;
    [SerializeField] private GameObject contentWrapper;

    public Sprite Sprite
    {
        set => faceRenderer.sprite = value;
    }

    public void ToggleVisibility(bool on)
    {
        gameObject.SetActive(on);
    }

    public void ToggleContent(bool on)
    {
        if (contentWrapper != null)
            contentWrapper.SetActive(on);
    }
}