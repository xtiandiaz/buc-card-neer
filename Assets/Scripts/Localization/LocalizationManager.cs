using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public interface ILocalizationManager : IInitializable, IDisposable
{
    void SetLanguage(Language toValue);
    IEnumerable<LanguageInfo> GetSupportedLanguages();
}

public interface ILocalizator
{
    string GetText(string forKey, params object[] andTidbits);
    
    void Hook(Text textField, string withKey, params object[] andTidbits);
    void Hook(TextMeshProUGUI textField, string withKey, params object[] andTidbits);
    void Hook(ButtonText button, string withKey, params object[] andTidbits);
}

public class LocalizationManager : ILocalizationManager, ILocalizator
{
    private readonly ReactiveProperty<Language?> currentLanguage = new ReactiveProperty<Language?>();
    private readonly Dictionary<string, string> localizedText = new Dictionary<string, string>();
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    
    private readonly ILocalizationCatalog catalog;
    private readonly IPlayerSettings playerSettings;

    private LocalizationManager(
        ILocalizationCatalog catalog,
        IPlayerSettings playerSettings
        )
    {
        this.catalog = catalog;
        this.playerSettings = playerSettings;

        this.catalog.Index();
    }

    public void Initialize()
    {
        disposables.Add(playerSettings.WhenLanguageChanged
            .Subscribe(SetLanguage));
    }

    public void SetLanguage(Language toValue)
    {
        LoadData(catalog[toValue].DataFile);

        currentLanguage.Value = toValue;
    }

    public IEnumerable<LanguageInfo> GetSupportedLanguages()
    {
        return catalog.GetAll();
    }

    public string GetText(string forKey, params object[] andTidbits)
    {
        return !localizedText.ContainsKey(forKey) 
            ? "*****" 
            : string.Format(localizedText[forKey], andTidbits);
    }

    public void Hook(TextMeshProUGUI textField, string withKey, params object[] andTidbits)
    {
        GetObservableEntry(withKey)
            .TakeUntilDestroy(textField)
            .Subscribe(textField.SetText)
            .AddTo(textField);
    }

    public void Hook(Text textField, string withKey, params object[] andTidbits)
    {
        GetObservableEntry(withKey)
            .TakeUntilDestroy(textField)
            .SubscribeToText(textField)
            .AddTo(textField);
    }
    
    public void Hook(ButtonText button, string withKey, params object[] andTidbits)
    {
        GetObservableEntry(withKey)
            .Subscribe(button.SetText)
            .AddTo(button);
    }

    public void Dispose()
    {
        disposables.Dispose();
    }

    private IObservable<string> GetObservableEntry(string withKey, params object[] andTidbits)
    {
        return currentLanguage
            .Where(lang => lang.HasValue)
            .Select(_ => GetText(withKey, andTidbits));
    }

    private void LoadData(TextAsset fromFileAsset)
    {
        var loadedData = JsonUtility.FromJson<LocalizationData>(fromFileAsset.text);
        
        localizedText.Clear();

        foreach (var entry in loadedData.entries)
            localizedText.Add(entry.key, entry.value);
    }

    [Serializable]
    private class LocalizationData
    {
        public LocalizationEntry[] entries = default;
    }
    
    [Serializable]
    public class LocalizationEntry
    {
        public string key;
        public string value;
    }
}