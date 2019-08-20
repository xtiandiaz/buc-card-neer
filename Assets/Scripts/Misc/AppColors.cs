using UnityEngine;

public interface IAppColors
{
}

[CreateAssetMenu(menuName = "Model/Misc/Game Colors")]
public class AppColors : ScriptableObject, IAppColors
{
    public static readonly Color ToggleOn = new Color32(32, 191, 107,255);
    public static readonly Color ToggleOff = new Color32(165, 177, 194, 255);
}