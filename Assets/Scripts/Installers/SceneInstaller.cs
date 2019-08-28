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
    
    [Header("Models")] 
    [SerializeField] private ArtificeCatalog artificeCatalog = default;

    public override void InstallBindings()
    {
        #region Viewport
        
        Container.BindInterfacesTo<GameCamera>().FromComponentInNewPrefab(cameraPrefab).AsSingle().NonLazy();
        Container.Bind<Viewport>().FromResolveGetter<IViewportProvider>(vpProv => vpProv.GetViewport(0))
            .AsSingle();
        
        Container.Bind<EventSystem>().FromComponentInNewPrefab(eventSystemPrefab).AsSingle().NonLazy();

        #endregion
        
        #region Cards
        
        Container.BindFactory<ICardModel, ICardView, Card, Card.Factory>().AsSingle();
        Container.BindFactory<IPlayerCardModel, IPlayerCardView, PlayerCard, PlayerCard.Factory>().AsSingle();
        Container.BindFactory<ICardModel, IMerchantCardView, MerchantCard, MerchantCard.Factory>().AsSingle();
        Container.BindFactory<IArtificeCardModel, IArtificeCardView, ArtificeCard, ArtificeCard.Factory>().AsSingle();

        Container.BindInterfacesTo<ArtificeCatalog>().FromInstance(artificeCatalog).WhenInjectedInto<CardFactory>();
        Container.BindInterfacesTo<CardFactory>().AsSingle();

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