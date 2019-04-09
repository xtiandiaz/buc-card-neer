using System;
using UnityEngine;
using Zenject;

public class AbilityCardView : CardView
{
    public class Factory : PlaceholderFactory<string, IAbilityCard, AbilityCardView>
    {
    }
    
    [SerializeField] private SpriteRenderer iconRenderer;
    private new IAbilityCard card;

    protected override void Initialize()
    {
        card = (IAbilityCard) base.card;

        iconRenderer.color = palette.GetColor(card.AbilityType);
    }
}