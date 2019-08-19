using System;

[Flags]
public enum DeviceType
{
    None              = 0,
    Catapult          = 1,
    MidasTouch        = 1 << 2,
    TraderSpell       = 1 << 3
}