using System;
using UniRx;
using UnityEngine;
using Zenject;

public class PlayerCardView : CardView
{
    public class Factory : PlaceholderFactory<string, PlayerCardView>
    {
    }

    [SerializeField] private CardStatView healthStat;
    [SerializeField] private CardStatView staminaStat;
    [SerializeField] private CardStatView defenseStat;
}