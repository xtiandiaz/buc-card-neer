using System;
using UniRx;
using UnityEngine;
using Zenject;

public interface IUserSettings : IInitializable, IDisposable
{
    Language Language { get; set; }
    bool ShouldPlayAudio { get; set; }
    bool ShouldDealDeviceCards { get; set; }
    
    IObservable<Language> WhenLanguageChanged { get; }
}

public class UserSettings : IUserSettings
{
    private const string LanguagePrefKey = "Language";
    private const string AudioPrefKey = "ShouldPlayAudio";
    private const string DeviceCardsPrefKey = "ShouldDealDeviceCards";

    private readonly BehaviorSubject<Language> languageSelection;
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    
    private UserSettings()
    {
        languageSelection = new BehaviorSubject<Language>(Language);
    }
    
    public Language Language
    {
        get => GetEnum(LanguagePrefKey, Language.English);
        set
        {
            SetEnum(LanguagePrefKey, value);
            languageSelection.OnNext(value);
        }
    }

    public bool ShouldPlayAudio
    {
        get => GetBool(AudioPrefKey, true);
        set => SetBool(AudioPrefKey, value);
    }

    public bool ShouldDealDeviceCards
    {
        get => GetBool(DeviceCardsPrefKey);
        set => SetBool(DeviceCardsPrefKey, value);
    }
    
    public IObservable<Language> WhenLanguageChanged => languageSelection.DistinctUntilChanged();

    public void Initialize()
    {
        disposables.Add(Observable.EveryApplicationPause()
            .Where(wasPaused => wasPaused)
            .Subscribe(_ => PlayerPrefs.Save()));
    }
    
    public void Dispose()
    {
        languageSelection.Dispose();
        
        disposables.Dispose();
    }

    private static T GetEnum<T>(string withKey, T andDefaultValue) where T : struct, IConvertible
    {
        return (T)(object)PlayerPrefs.GetInt(withKey,Convert.ToInt32(andDefaultValue));
    }

    private static T GetEnum<T>(string withKey) where T : struct, IConvertible
    {
        return (T)(object)PlayerPrefs.GetInt(withKey);
    }

    private static void SetEnum<T>(string withKey, T andValue) where T : struct, IConvertible
    {
        PlayerPrefs.SetInt(withKey, Convert.ToInt32(andValue));
    }

    private static bool GetBool(string withKey, bool andDefaultValue = false)
    {
        return PlayerPrefs.GetInt(withKey, andDefaultValue ? 1 : 0) == 1;
    }

    private static void SetBool(string withKey, bool andValue)
    {
        PlayerPrefs.SetInt(withKey, andValue ? 1 : 0);
    }
}