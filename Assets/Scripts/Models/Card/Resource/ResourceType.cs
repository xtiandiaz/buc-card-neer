using System;

[Flags]
public enum ResourceType
{
    None                = 0,
    Food                = CardType.Food,
    Artifact            = CardType.Artifact,
    Gem                 = CardType.Gem,
    WeaponMelee         = CardType.WeaponMelee,
    WeaponRanged        = CardType.WeaponRanged,
    Medicine            = CardType.Medicine,
    
    Item                = Food | Artifact | Gem,
    Tool                = WeaponMelee | WeaponRanged | Medicine
}