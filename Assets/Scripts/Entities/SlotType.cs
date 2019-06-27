using System;

[Flags]
public enum SlotType
{
    Supply    = 1 << 0,
    Boarding  = 1 << 1,
    Player    = 1 << 2,
    Storage   = 1 << 4,
    Mount     = 1 << 5,
    
    Stash = Storage | Mount
}