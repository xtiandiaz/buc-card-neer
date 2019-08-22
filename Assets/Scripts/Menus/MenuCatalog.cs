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
    [SerializeField] private GameOverMenu gameOver = default;
    [SerializeField] private StageFinishedMenu stageFinished = default;
    
    private Dictionary<Type, Menu> index;

    public void Index()
    {
        if (index != null)
            return;

        index = new Dictionary<Type, Menu>
        {
            [typeof(IPauseMenu)] = pause, 
            [typeof(ISettingsMenu)] = settings,
            [typeof(ILanguageSelectionMenu)] = languageSelection,
            [typeof(IStoreMenu)] = store,
            [typeof(ILogbookMenu)] = logbook,
            [typeof(IGameOverMenu)] = gameOver,
            [typeof(IStageFinishedMenu)] = stageFinished,
        };
    }

    public Menu GetPrefab<T>() where T : IMenu
    {
        return index[typeof(T)];
    }
}