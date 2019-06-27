using UnityEngine;

public interface IPlayerCardModel : ICardModel
{
    int MaxHealthPoints { get; }
    int InitialCoins { get; }
}

[CreateAssetMenu(menuName = "Model/PlayerCard")]
public class PlayerCardModel : CardModel, IPlayerCardModel
{
    [SerializeField] private int maxHealthPoints = 100;
    [SerializeField] private int initialCoins = 100;
    
    public override CardType Type => CardType.Player;
    public int MaxHealthPoints => maxHealthPoints;
    public int InitialCoins => initialCoins;
}