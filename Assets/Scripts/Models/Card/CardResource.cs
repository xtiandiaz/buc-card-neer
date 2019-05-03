using System;
using UnityEngine;

[Flags]
public enum ResourceType
{
    None                = 0, 
    Food                = 1 << 0, 
    Artifact            = 1 << 1,
    Gem                 = 1 << 2,
    ArtilleryWeapon     = 1 << 3,
    MeleeWeapon         = 1 << 4,
    Money               = 1 << 5
}

public interface IResourceCard : ICard
{
    ResourceType ResourceType { get; }
}

[CreateAssetMenu(fileName = "CardResource", menuName = "Game/Card/Resource", order = 1)]
public class CardResource : Card, IResourceCard
{
    [SerializeField] private ResourceType type;
    [SerializeField] private int value;

    public override int Value => value;
    public override CardType Type => CardType.Resource;
    public override CardType InteractionMask => CardType.Pirate | CardType.Merchant | CardType.Player;
    public ResourceType ResourceType => type;

}