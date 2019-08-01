using UnityEngine;
using Zenject;

public class AppInstaller : MonoInstaller
{
    [Header("Data Models")]
    [SerializeField] private BoardModel board = default;
    [SerializeField] private MenuCatalog menuCatalog = default;
    
    [Header("Audio")] 
    [SerializeField] private AudioRepository audioRepository = default;
    [SerializeField] private AudioSource audioSourcePrefab = default;

    public override void InstallBindings()
    {
        Container.BindInterfacesTo<AppController>().AsSingle();

        #region Menus

        Container.Bind<IMenuCatalog>().FromInstance(menuCatalog).AsSingle();

        #endregion
        
        #region Board

        Container.BindInterfacesTo<BoardModel>().FromInstance(board).AsSingle();

        #endregion
        
        #region Audio

        Container.BindInterfacesTo<AudioRepository>().FromInstance(audioRepository).AsSingle().NonLazy();
        Container.BindInterfacesTo<AudioManager>().AsSingle();
        Container.BindMemoryPool<AudioSource, AudioSourcePool>()
            .WithMaxSize(16)
            .FromComponentInNewPrefab(audioSourcePrefab)
            .AsSingle();

        #endregion
    }
}