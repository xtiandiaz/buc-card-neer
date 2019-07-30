using System;

[Flags]
public enum CardType
{
    Player             = 1 << 0,
    Pirate             = 1 << 1,
    Merchant           = 1 << 2,
    
    Inspector          = 1 << 3,
    Monster            = 1 << 3,
    
    Food               = 1 << 4,
    Artifact           = 1 << 5,
    Gem                = 1 << 6,
    WeaponMelee        = 1 << 7,
    WeaponRanged       = 1 << 8,
    Medicine           = 1 << 9,
    Trap               = 1 << 10,
                         
    Item               = Food | Gem | Artifact,
    Tool               = WeaponMelee | WeaponRanged | Medicine | Trap,
    Resource           = Item | Tool,
    Agent              = Pirate | Merchant | Inspector
}