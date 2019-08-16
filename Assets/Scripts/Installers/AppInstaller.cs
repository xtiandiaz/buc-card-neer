using UnityEngine;
using Zenject;

public class AppInstaller : MonoInstaller
{
    [Header("Data Models")]
    [SerializeField] private BoardModel board = default;
    [SerializeField] private MenuCatalog menuCatalog = default;

    public override void InstallBindings()
    {
        Container.BindInterfacesTo<AppController>().AsSingle();

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