using System;
using UniRx;
using UnityEngine;
using Zenject;

public class PlayerCardView : CardView
{
    public class Factory : PlaceholderFactory<string, IPlayerCard, PlayerCardView>
    {
    }

    [SerializeField] private CardStatView healthStat;
    [SerializeField] private CardStatView staminaStat;
    [SerializeField] private CardStatView defenseStat;
    [SerializeField] private SpriteRenderer[] abilityIconRenderers;
    
    private new IPlayerCard card;

    protected override string DefaultSortingLayer => settings.PlayerDefaultSortingLayerName;
    protected override string DefaultTextSortingLayer => settings.PlayerTextDefaultSortingLayerName;

    protected override void Initialize()
    {
        card = (IPlayerCard) base.card;
        
        healthStat.Observe(card.ObservableHealth);
        staminaStat.Observe(card.ObservableStamina);
        defenseStat.Observe(card.ObservableDefense);

        card.AcquiredAbility.Subscribe(ability =>
            {
                var (abilityType, abilityIndex) = ability;
                abilityIconRenderers[abilityIndex].color = palette.GetColor(abilityType);
            })
            .AddTo(this);
    }
}