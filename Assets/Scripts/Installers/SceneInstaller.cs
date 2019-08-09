using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    [Header("Viewport")]
    [SerializeField] private GameCamera cameraPrefab = default;
    [SerializeField] private EventSystem eventSystemPrefab = default;
    
    [Header("Audio")] 
    [SerializeField] private AudioRepository audioRepository = default;
    [SerializeField] private AudioSource audioSourcePrefab = default;

    public override void InstallBindings()
    {
        #region Viewport
        
        Container.BindInterfacesTo<GameCamera>().FromComponentInNewPrefab(cameraPrefab).AsSingle().NonLazy();
        Container.Bind<Viewport>().FromResolveGetter<IViewportProvider>(vpProv => vpProv.GetViewport(0))
            .AsSingle();
        
        Container.Bind<EventSystem>().FromComponentInNewPrefab(eventSystemPrefab).AsSingle().NonLazy();

        #endregion

        #region Menus

        Container.BindInterfacesTo<MenuFactory>().AsSingle();

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