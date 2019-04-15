using System;
using UnityEngine;
using Zenject;

public class ItemCardView : CardView
{
    public class Factory : PlaceholderFactory<string, IItemCard, ItemCardView>
    {
    }
    
    [SerializeField] private TextMesh textMesh;
    [SerializeField] private SpriteRenderer iconRenderer;
    [SerializeField] private Sprite healthIcon;
    [SerializeField] private Sprite staminaIcon;
    [SerializeField] private Sprite defenseIcon;

    private new IItemCard card;

    protected override void Initialize()
    {
        card = (IItemCard) base.card;
    }

    protected override void Awake()
    {
        base.Awake();
        
        textMesh.text = $"{card.Value}";
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