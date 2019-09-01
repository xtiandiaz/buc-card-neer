using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameInstaller : SceneInstaller
{
    [Header("Models")]
    [SerializeField] private DeckModelGrouped deck = default;
    [Space] 
    [SerializeField] private PlayerCardModel player = default;
    [Space]
    [SerializeField] private SlotModel supplySlot = default;
    [Space]
    [SerializeField] private SlotModel plank = default;
    [SerializeField] private SlotModel helm = default;
    [SerializeField] private SlotModel storage = default;
    [SerializeField] private SlotModel mount = default;
    [Space]
    [InjectOptional]
    [SerializeField] private StageModel stage = default;

    [Header("Views")] 
    [SerializeField] private BoardView boardViewPrefab = default;

    [Header("UI")]
    [SerializeField] private GameMenu gameMenuPrefab = default;
    [SerializeField] private BoardMenu boardMenuPrefab = default;
    [Space] 
    [SerializeField] private FloatingBannerModelCatalog floatingBannerCatalog = default;
    [SerializeField] private FloatingBanner floatingBannerPrefab = default;

    public override void InstallBindings()
    {
        base.InstallBindings();

        #region Controllers

        Container.BindInterfacesTo<RoutingController>().AsSingle().NonLazy();
        Container.BindInterfacesTo<DealingController>().AsSingle().NonLazy();
        Container.BindInterfacesTo<DefermentController>().AsSingle().NonLazy();
        Container.BindInterfacesTo<ForwardingController>().AsSingle().NonLazy();
        Container.BindInterfacesTo<DismissalController>().AsSingle().NonLazy();
        Container.BindInterfacesTo<MatchingController>().AsSingle().NonLazy();
        Container.BindInterfacesTo<ArtificeMatchingController>().AsSingle().NonLazy();
        Container.BindInterfacesTo<ConfrontationController>().AsSingle().NonLazy();
        Container.BindInterfacesTo<LodgingController>().AsSingle().NonLazy();
        Container.BindInterfacesTo<ClashingController>().AsSingle().NonLazy();
        Container.BindInterfacesTo<ShootingController>().AsSingle().NonLazy();
        Container.BindInterfacesTo<GameController>().AsSingle().NonLazy();
        Container.BindInterfacesTo<GameStatusController>().AsSingle().NonLazy();
        
        Container.BindExecutionOrder<GameAudioController>(100);
        Container.BindInterfacesAndSelfTo<GameAudioController>().AsSingle();
        
        Container.BindInterfacesTo<CardNotificationsController>().AsSingle().NonLazy();
        Container.BindInterfacesTo<VisualEffectsController>().AsSingle().NonLazy();

        #endregion

        #region Stage

        Container.BindInterfacesAndSelfTo<IStage>()
            .FromResolveGetter<Stage.Factory, IStage>(stageFactory => (IStage) stageFactory.Create(stage))
            .AsSingle();

        #endregion
        
        #region Board
        
        Container.Bind<IBoardView>().FromComponentInNewPrefab(boardViewPrefab).AsSingle();

        Container.Bind<ISlotModel>().FromInstance(supplySlot).WhenInjectedInto<IBoardFactory>();

        Container.Bind<ISlotModel>().FromInstance(helm).WhenInjectedInto<ShipFactory>();
        Container.Bind<ISlotModel>().FromInstance(plank).WhenInjectedInto<ShipFactory>();
        Container.Bind<ISlotModel>().FromInstance(storage).WhenInjectedInto<ShipFactory>();
        Container.Bind<ISlotModel>().FromInstance(mount).WhenInjectedInto<ShipFactory>();
        
        Container.BindFactory<IEnumerable<ISlot>, ISeaView, Sea, Sea.Factory>().AsSingle();
        
        Container.BindFactory<ISlot, ISlot, ISlot, ISlot, IShipView, Ship, Ship.Factory>()
            .AsSingle();
        Container.BindInterfacesTo<ShipFactory>().AsSingle();
        
        Container.BindFactory<ISea, IShip, IBoardModel, Board, Board.Factory>().AsSingle();
        Container.Bind<IBoard>().FromFactory<BoardFactory>().AsSingle();

        #endregion
        
        #region Deck
        
        Container.BindFactory<List<ICardModel>, Deck, Deck.Factory>().AsSingle();
        Container.Bind<IDeckFactory>().To<DeckFactory>().AsSingle();
        Container.Bind(typeof(IDeck))
            .FromResolveGetter<IDeckFactory, IDeck>(x => x.Create(deck)).AsSingle();
        
        #endregion
        
        #region Slots

        Container.BindFactory<ISlotModel, ISlotView, Slot, Slot.Factory>().AsSingle();
        Container.BindFactory<ISlotModel, IStashSlotView, StashSlot, StashSlot.Factory>().AsSingle();
        Container.BindInterfacesTo<SlotFactory>().AsSingle();
        
        #endregion
        
        #region Cards

        Container.BindInterfacesAndSelfTo<IPlayerCard>()
            .FromResolveGetter<ICardFactory, IPlayerCard>(cardFactory => (IPlayerCard) cardFactory.Create(player))
            .AsSingle();

        #endregion

        #region UI
        
        Container.BindInterfacesTo<GameMenu>().FromComponentInNewPrefab(gameMenuPrefab).AsSingle().NonLazy();
        Container.BindInterfacesTo<BoardMenu>().FromComponentInNewPrefab(boardMenuPrefab).AsSingle().NonLazy();

        Container.BindFactory<FloatingBanner, FloatingBanner.Factory>().FromComponentInNewPrefab(floatingBannerPrefab);
        Container.BindInterfacesAndSelfTo<FloatingBannerModelCatalog>().FromInstance(floatingBannerCatalog)
            .WhenInjectedInto<FloatingBannerFactory>();
        Container.BindInterfacesTo<FloatingBannerFactory>().AsSingle();

        #endregion
    }
}
