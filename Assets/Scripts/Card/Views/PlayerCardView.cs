using UnityEngine;

public interface IPlayerCardView : ICardView
{
    int MaxHealth { set; }
    int Coins { set; }
}

public class PlayerCardView : CardView, IPlayerCardView
{
    [SerializeField] private CardValue coins = default;

    public int MaxHealth { private get; set; }
    
    public int Coins
    {
        set => coins.SetValue(value);
    }

    public override int Value
    {
        set => customizer.StringValue = $"{value}<size=3.5> / {MaxHealth}</size>"; 
    }
}