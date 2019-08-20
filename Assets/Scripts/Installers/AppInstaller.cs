using UnityEngine;
using Zenject;

public class AppInstaller : MonoInstaller
{
    [SerializeField] private BoardModel board = default;
    [SerializeField] private MenuCatalog menuCatalog = default;
    [SerializeField] private LocalizationCatalog localizationCatalog = default;

    public override void InstallBindings()
    {
        Container.BindInterfacesTo<AppController>().AsSingle();
        Container.BindInterfacesTo<UserSettings>().AsSingle().NonLazy();

        #region Menus

        Container.Bind<IMenuCatalog>().FromInstance(menuCatalog).AsSingle();

        #endregion
        
        #region Board

        Container.BindInterfacesTo<BoardModel>().FromInstance(board).AsSingle();

        #endregion

        #region Localization
        
        Container.BindInterfacesTo<LocalizationCatalog>().FromInstance(localizationCatalog).AsSingle();
        Container.BindInterfacesTo<LocalizationManager>().AsSingle();

        #endregion
    }
}