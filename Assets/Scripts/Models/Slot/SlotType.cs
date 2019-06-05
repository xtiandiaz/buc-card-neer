using System;

[Flags]
public enum SlotType
{
    Supply    = 1 << 0,
    Boarding  = 1 << 1,
    Storage   = 1 << 2,
    Player    = 1 << 3
}