using UnityEngine;

public enum ResourceType
{
    Food, 
    Artifact,
    Gem,
    ArtilleryWeapon,
    MeleeWeapon,
    Money
}

[CreateAssetMenu(fileName = "CardResource", menuName = "Game/Card/Resource", order = 1)]
public class CardResource : Card
{
    [SerializeField] private ResourceType type;
    [SerializeField] private int value;

    public override int Value => value;
    public override CardType Type => CardType.Resource;
    public override CardType InteractionMask => CardType.Pirate | CardType.Merchant | CardType.Player;
    public ResourceType ResourceType => type;

}