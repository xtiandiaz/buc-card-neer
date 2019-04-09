using UnityEngine;
using Zenject;

public class BaddieCardView : CardView
{
    public class Factory : PlaceholderFactory<string, IBaddieCard, BaddieCardView>
    {
    }

    [SerializeField] private CardStatView staminaStat;
    [SerializeField] private CardStatView attackStat;

    private new IBaddieCard card;
    
    protected override void Initialize()
    {
        card = (IBaddieCard) base.card;
        
        staminaStat.Observe(card.ObservableStamina);
        attackStat.Observe(card.ObservableAttack);
    }
}