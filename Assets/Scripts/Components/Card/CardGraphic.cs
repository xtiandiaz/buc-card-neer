using System.Collections.Generic;
using UnityEngine;

public class CardGraphic : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> spriteRenderers;

    public Sprite Sprite
    {
        set => spriteRenderers.ForEach(r => r.sprite = value);
    }

    public Color Color
    {
        set => spriteRenderers.ForEach(r => r.color = value);
    }
}