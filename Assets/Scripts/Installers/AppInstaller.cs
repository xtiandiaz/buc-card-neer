using UnityEngine;
using Zenject;

public class AppInstaller : MonoInstaller
{
    [Header("Models")]
    [SerializeField] private BoardModel board = default;
    [SerializeField] private StageModel defaultStage = default;

    [Header("Catalogs")]
    [SerializeField] private MenuCatalog menuCatalog = default;
    [SerializeField] private LocalizationCatalog localizationCatalog = default;

    public override void InstallBindings()
    {
        Container.BindInterfacesTo<AppController>().AsSingle();

        #region Board

        Container.BindInterfacesTo<BoardModel>().FromInstance(board).AsSingle();
        Container.BindInterfacesTo<BoardLayout>().AsSingle();

        #endregion

        #region Stage

        Container.BindFactory<IStageModel, Stage, Stage.Factory>().AsSingle();
        Container.Bind<IStageModel>().FromInstance(defaultStage).WhenInjectedInto<AppController>();

        #endregion

        #region Player
        
        Container.BindInterfacesTo<PlayerSettings>().AsSingle().NonLazy();
        Container.BindInterfacesTo<PlayerStats>().AsSingle().NonLazy();

        #endregion

        #region Menus

        Container.Bind<IMenuCatalog>().FromInstance(menuCatalog).AsSingle();

        #endregion

        #region Localization
        
        Container.BindInterfacesTo<LocalizationCatalog>().FromInstance(localizationCatalog).AsSingle();
        Container.BindInterfacesTo<LocalizationManager>().AsSingle();
        Container.Bind<Localizator>().AsSingle().NonLazy();

        #endregion
    }
}