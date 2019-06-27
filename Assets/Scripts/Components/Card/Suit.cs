using System.Collections.Generic;
using UnityEngine;

public class Suit : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> iconRenderers;

    public void Customize(ISuitModel fromModel)
    {
        iconRenderers.ForEach(sRenderer =>
        {
            sRenderer.sprite = fromModel.Icon;
            sRenderer.color = fromModel.Color;
        });
    }
}