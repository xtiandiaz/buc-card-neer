using System;
using System.Collections.Generic;
using UnityEngine;

public enum Language
{
    English,
    Spanish
}

[Serializable]
public class LanguageInfo
{
    [SerializeField] private Language key = default;
    [SerializeField] private string name = default;
    [SerializeField] private TextAsset dataFile = default;

    public Language Key => key;
    public string Name => name;
    public TextAsset DataFile => dataFile;
}

public interface ILocalizationCatalog
{
    LanguageInfo this[Language key] { get; }
    
    void Index();
    IEnumerable<LanguageInfo> GetAll();
}

[CreateAssetMenu(menuName = "Model/Localization/Catalog")]
public class LocalizationCatalog : ScriptableObject, ILocalizationCatalog
{
    [SerializeField] private LanguageInfo[] languages = default;

    private Dictionary<Language, LanguageInfo> dataIndex;
    
    public LanguageInfo this[Language key] => dataIndex[key];

    public void Index()
    {
        if (dataIndex != null)
            return;
        
        dataIndex = new Dictionary<Language, LanguageInfo>();

        foreach (var language in languages)
            dataIndex[language.Key] = language;
    }

    public IEnumerable<LanguageInfo> GetAll()
    {
        return languages;
    }
}