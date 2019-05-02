using UnityEngine;

public static class ColorExtensions
{
    public static Color SetAlpha(this Color color, float toValue)
    {
        return new Color(color.r, color.g, color.b, toValue);
    }
    
    public static Color Tint(this Color color, Color withColor, float byFactor)
    {
        return Color.LerpUnclamped(color, withColor, byFactor);
    }
}