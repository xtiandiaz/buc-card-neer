using System;
using UnityEngine;

[Flags]
public enum ResourceType
{
    None                = 0,
    Food                = CardType.Food,
    Artifact            = CardType.Artifact,
    Gem                 = CardType.Gem,
    ArtilleryWeapon     = CardType.ArtilleryWeapon,
    MeleeWeapon         = CardType.MeleeWeapon,
    Money               = CardType.Money
}

public interface IResourceCard : ICard
{
    ResourceType ResourceType { get; }
    Sprite Item { get; }
    ISuit Suit { get; }
    bool IsTreasure { get; }
    bool WasPaidFor { get; set; }
}

[CreateAssetMenu(fileName = "CardResource", menuName = "Game/Card/Resource", order = 1)]
public class CardResource : Card, IResourceCard
{
    [SerializeField] private int value;
    [SerializeField] private Sprite item;
    [SerializeField] private Suit suit;
    [SerializeField] private bool isTreasure;

    public override int Value => value;
    public override CardType Type => (CardType) ResourceType;
    public override CardType InteractionMask => CardType.Pirate | CardType.Merchant | CardType.Player;
    
    public ResourceType ResourceType => suit.ResourceType;
    public Sprite Item => item;
    public ISuit Suit => suit;
    public bool IsTreasure => isTreasure;
    public bool WasPaidFor { get; set; }
}