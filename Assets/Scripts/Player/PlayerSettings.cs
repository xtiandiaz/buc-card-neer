using System;
using UniRx;
using Zenject;

public interface IPlayerSettings : IPlayerData, IInitializable, IDisposable
{
    Language Language { get; set; }
    bool ShouldPlayAudio { get; set; }
    bool ShouldDealDeviceCards { get; set; }
    
    IObservable<Language> WhenLanguageChanged { get; }
}

public class PlayerSettings : PlayerData, IPlayerSettings
{
    private const string LanguagePrefKey = "Language";
    private const string AudioPrefKey = "ShouldPlayAudio";
    private const string DeviceCardsPrefKey = "ShouldDealDeviceCards";

    private readonly BehaviorSubject<Language> languageSelection;
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    
    private PlayerSettings()
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
            .Subscribe(_ => Save()));
    }
    
    public void Dispose()
    {
        languageSelection.Dispose();
        
        disposables.Dispose();
    }

    public override void Clear()
    {
        Delete(LanguagePrefKey);
        Delete(AudioPrefKey);
        Delete(DeviceCardsPrefKey);
    }
}