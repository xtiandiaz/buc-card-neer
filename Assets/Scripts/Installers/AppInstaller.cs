using UnityEngine;
using Zenject;

public class AppInstaller : MonoInstaller
{
    [Header("Models")]
    [SerializeField] private BoardModel board = default;
    [SerializeField] private MenuCatalog menuCatalog = default;
    [SerializeField] private UserSettings userSettings = default;

    public override void InstallBindings()
    {
        Container.BindInterfacesTo<AppController>().AsSingle();
        Container.BindInterfacesTo<UserSettings>().FromInstance(userSettings).AsSingle();

        #region Menus

        Container.Bind<IMenuCatalog>().FromInstance(menuCatalog).AsSingle();

        #endregion
        
        #region Board

        Container.BindInterfacesTo<BoardModel>().FromInstance(board).AsSingle();

        #endregion

        #region Localization

        Container.BindInterfacesTo<LocalizationProvider>().AsSingle();

        #endregion
    }
}