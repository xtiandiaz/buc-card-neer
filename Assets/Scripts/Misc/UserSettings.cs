using UnityEngine;

public interface IUserSettings
{
    bool ShouldPlayAudio { get; set; }
    
    bool ShouldDealDeviceCards { get; set; }
}

[CreateAssetMenu(menuName = "Model/Misc/User Settings")]
public class UserSettings : ScriptableObject, IUserSettings
{
    [Header("General")]
    [SerializeField] private bool shouldPlayAudio = true;

    [Header("Game")] 
    [SerializeField] private bool shouldDealDeviceCards = false;

    public bool ShouldPlayAudio
    {
        get => shouldPlayAudio;
        set => shouldPlayAudio = value;
    }

    public bool ShouldDealDeviceCards
    {
        get => shouldDealDeviceCards;
        set => shouldDealDeviceCards = value;
    }
}