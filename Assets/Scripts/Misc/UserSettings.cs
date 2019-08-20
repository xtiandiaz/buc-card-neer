using System;
using UniRx;
using UnityEngine;

public interface IUserSettings
{
    Language Language { get; set; }
    IObservable<Language> WhenLanguageChanged { get; }
    
    bool ShouldPlayAudio { get; set; }
    
    bool ShouldDealDeviceCards { get; set; }
}

[CreateAssetMenu(menuName = "Model/Misc/User Settings")]
public class UserSettings : ScriptableObject, IUserSettings
{
    private readonly ReactiveProperty<Language> language = new ReactiveProperty<Language>(Language.English);
    
    [Header("General")]
    [SerializeField] private bool shouldPlayAudio = true;

    [Header("Game")] 
    [SerializeField] private bool shouldDealDeviceCards = false;

    public Language Language
    {
        get => language.Value;
        set => language.Value = value;
    }

    public IObservable<Language> WhenLanguageChanged => language.DistinctUntilChanged();
    
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

    private void OnDestroy()
    {
        language.Dispose();
    }
}