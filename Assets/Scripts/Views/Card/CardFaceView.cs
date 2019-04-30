using UnityEngine;

public interface ICardFaceView
{
    Sprite Sprite { set; }
    void ToggleVisibility(bool on);
}

public class CardFaceView : MonoBehaviour, ICardFaceView
{
    [SerializeField] private SpriteRenderer faceRenderer;

    public Sprite Sprite
    {
        set => faceRenderer.sprite = value;
    }

    public void ToggleVisibility(bool on)
    {
        gameObject.SetActive(on);
    }
}