using UnityEngine;
using Zenject;

public class PirateCardView : CardView
{
    public class Factory : PlaceholderFactory<string, PirateCardView>
    {
    }

    [SerializeField] private CardStatView staminaStat;
    [SerializeField] private CardStatView attackStat;
    
    protected override void Initialize()
    {
    }
}