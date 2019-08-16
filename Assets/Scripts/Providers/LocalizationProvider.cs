using System;

public interface ILocalizationProvider
{
    string GetName(DeviceType forType);
}

public class LocalizationProvider : ILocalizationProvider
{
    public string GetName(DeviceType forType)
    {
        switch (forType)
        {
            case DeviceType.Catapult:
                return "Catapult";
            case DeviceType.MidasTouch:
                return "Midas Touch";
            case DeviceType.TraderSpell:
                return "Trader’s Spell";
            default:
                throw new ArgumentOutOfRangeException(nameof(forType), forType, null);
        }
    }
}