using UnityEngine;

[CreateAssetMenu(fileName = "CardFoe", menuName = "Game/Card Foe", order = 1)]
public class CardFoe : Card
{
    [SerializeField] private uint hitPoints;
    
    public override void Initialize()
    {
        base.Initialize(CardType.Foe);
    }
}