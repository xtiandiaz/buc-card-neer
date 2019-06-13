using System;

[Flags]
public enum CardType
{
    Player             = 1 << 0,
    Pirate             = 1 << 1,
    Merchant           = 1 << 2,
    Inspector          = 1 << 3,
    Food               = 1 << 4,
    Artifact           = 1 << 5,
    Gem                = 1 << 6,
    WeaponMelee        = 1 << 7,
    WeaponRanged       = 1 << 8,
    Medicine           = 1 << 9,
    Trap               = 1 << 10,
                         
    Resource           = Food | Gem | Artifact | WeaponMelee | WeaponRanged | Medicine | Trap,
    Agent              = Pirate | Merchant | Inspector
}