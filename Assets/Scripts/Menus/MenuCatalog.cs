using System;
using System.Collections.Generic;
using UnityEngine;

public interface IMenuCatalog
{
    void Index();
    Menu GetPrefab<T>() where T : IMenu;
}

[CreateAssetMenu(menuName = "Model/UI/Menu Catalog")]
public class MenuCatalog : ScriptableObject, IMenuCatalog
{
    [SerializeField] private PauseMenu pause = default;
    [SerializeField] private SettingsMenu settings = default;
    [SerializeField] private LanguageSelectionMenu languageSelection = default;
    [SerializeField] private StoreMenu store = default;
    [SerializeField] private LogbookMenu logbook = default;
    
    private Dictionary<Type, Menu> prefabIndex;

    public void Index()
    {
        if (prefabIndex != null)
            return;

        prefabIndex = new Dictionary<Type, Menu>
        {
            [typeof(IPauseMenu)] = pause, 
            [typeof(ISettingsMenu)] = settings,
            [typeof(ILanguageSelectionMenu)] = languageSelection,
            [typeof(IStoreMenu)] = store,
            [typeof(ILogbookMenu)] = logbook
        };
    }

    public Menu GetPrefab<T>() where T : IMenu
    {
        return prefabIndex[typeof(T)];
    }
}