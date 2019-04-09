using System;
using UnityEngine;
using Zenject;

public class ResourceCardView : CardView
{
    public class Factory : PlaceholderFactory<string, IResourceCard, ResourceCardView>
    {
    }
    
    [SerializeField] private TextMesh textMesh;
    [SerializeField] private SpriteRenderer iconRenderer;
    [SerializeField] private Sprite healthIcon;
    [SerializeField] private Sprite staminaIcon;
    [SerializeField] private Sprite defenseIcon;

    private new IResourceCard card;

    protected override void Initialize()
    {
        card = (IResourceCard) base.card;
    }

    protected override void Awake()
    {
        base.Awake();
        
        textMesh.text = $"{card.Value}";
        frontFace.color = GetTypeColor();
        iconRenderer.sprite = GetIconSprite();
    }

    private Sprite GetIconSprite()
    {
        switch (card.Type)
        {
            case CardType.Health:
                return healthIcon;
            case CardType.Stamina:
                return staminaIcon;
            case CardType.Defense:
                return defenseIcon;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}