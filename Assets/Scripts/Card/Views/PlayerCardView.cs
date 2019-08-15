using TMPro;
using UnityEngine;

public interface IPlayerCardView : ICardView
{
    int MaxHealth { set; }
    int Coins { set; }
}

public class PlayerCardView : CardView, IPlayerCardView
{
    [SerializeField] private CardValue coins = default;
    [SerializeField] private TextMeshPro maxHealth = default;

    public int MaxHealth
    {
        set => maxHealth.text = $"{value}";
    }
    
    public int Coins
    {
        set => coins.SetValue(value);
    }
}