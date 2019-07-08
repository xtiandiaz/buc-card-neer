using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [Header("Prefabs")] 
    [SerializeField] private BoardView boardView = default;

    [Header("Data Models")] 
    [SerializeField] private BoardModel board = default;
    [Space]
    [SerializeField] private DeckModel deck = default;
    [Space] 
    [SerializeField] private PlayerCardModel player = default;
    [Space]
    [SerializeField] private List<SlotModel> supplySlots = default;
    [Space]
    [SerializeField] private SlotModel plank = default;
    [SerializeField] private SlotModel helm = default;
    [SerializeField] private SlotModel storage = default;
    [SerializeField] private SlotModel mount = default;

    [Header("Viewport")]
    [SerializeField] private new GameCamera camera = default;
    
    [Header("Views")]
    [SerializeField] private GameMenuView gameMenuView = default;

    [Header("Settings")] 
    [SerializeField] private CardAnimationSettings cardAnimationSettings = default;

    [Header("Audio")] 
    [SerializeField] private AudioRepository audioRepository = default;
    [SerializeField] private AudioSource audioSourcePrefab = default;

    public override void InstallBindings()
    {
        #region Viewport

        Container.BindInterfacesTo<GameCamera>().FromInstance(camera).AsSingle();
        camera.Initialize(board);

        Container.Bind<Viewport>().FromMethod(() => camera.GetViewport(0));

        #endregion

        #region Controllers

        Container.BindInterfacesTo<CardRouter>().AsSingle().NonLazy();
        Container.BindInterfacesTo<CardDealer>().AsSingle().NonLazy();
        Container.BindInterfacesTo<CardDeferrer>().AsSingle().NonLazy();
        Container.BindInterfacesTo<CardDismisser>().AsSingle().NonLazy();
        Container.BindInterfacesTo<CardMatcher>().AsSingle().NonLazy();
        Container.BindInterfacesTo<CardHost>().AsSingle().NonLazy();
        Container.BindInterfacesTo<CardClasher>().AsSingle().NonLazy();
        Container.BindInterfacesTo<CardShooter>().AsSingle().NonLazy();
        
        Container.BindInterfacesTo<GameController>().AsSingle().NonLazy();
        Container.BindInterfacesTo<BoardingController>().AsSingle().NonLazy();
        Container.BindInterfacesTo<ClashingController>().AsSingle().NonLazy();
        Container.BindInterfacesTo<SupplyController>().AsSingle().NonLazy();
        Container.BindInterfacesTo<CombatController>().AsSingle().NonLazy();

        #endregion
        
        #region UI
        
        Container.BindInterfacesTo<GameMenuView>().FromInstance(gameMenuView).AsSingle();

        #endregion
        
        #region Board
        
        Container.Bind<IBoardModel>().FromInstance(board).AsSingle();
        Container.Bind<IBoardView>().FromComponentInNewPrefab(boardView).AsSingle();

        #endregion

        #region Sea

        Container.BindFactory<IEnumerable<ISlot>, ISeaView, Sea, Sea.Factory>().AsSingle();
        Container.Bind<List<SlotModel>>().FromInstance(supplySlots).WhenInjectedInto<SeaFactory>();
        Container.Bind<ISea>().FromFactory<SeaFactory>().AsSingle();

        #endregion
        
        #region Ship

        Container.BindFactory<ISlot, ISlot, ISlot, ISlot, IShipView, Ship, Ship.Factory>()
            .AsSingle();
        Container.Bind<ISlotModel>().FromInstance(helm).WhenInjectedInto<ShipFactory>();
        Container.Bind<ISlotModel>().FromInstance(plank).WhenInjectedInto<ShipFactory>();
        Container.Bind<ISlotModel>().FromInstance(storage).WhenInjectedInto<ShipFactory>();
        Container.Bind<ISlotModel>().FromInstance(mount).WhenInjectedInto<ShipFactory>();
        Container.Bind<IShip>().FromFactory<ShipFactory>().AsSingle();

        #endregion
        
        #region Deck & Card Providers
        
        Container.BindFactory<List<ICardModel>, Deck, Deck.Factory>().AsSingle();
        Container.Bind<IDeckFactory>().To<DeckFactory>().AsSingle();
        Container.Bind(typeof(IDeck))
            .FromResolveGetter<IDeckFactory, IDeck>(x => x.Create(deck)).AsSingle();
        
        #endregion
        
        #region Slots

        Container.BindFactory<ISlotModel, ISlotView, Slot, Slot.Factory>().AsSingle();
        Container.BindInterfacesTo<SlotFactory>().AsSingle();
        
        #endregion
        
        #region Cards
        
        Container.BindFactory<ICardModel, ICardView, Card, Card.Factory>().AsSingle();
        Container.BindFactory<IPlayerCardModel, IPlayerCardView, PlayerCard, PlayerCard.Factory>().AsSingle();
        Container.BindInterfacesTo<CardFactory>().AsSingle();

        Container.BindInterfacesAndSelfTo<IPlayerCard>()
            .FromResolveGetter<ICardFactory, IPlayerCard>(x => (IPlayerCard) x.Create(player)).AsSingle();

        #endregion
        
        #region Settings

        Container.Bind<CardAnimationSettings>().FromInstance(cardAnimationSettings).AsSingle();

        #endregion

        #region Audio

        Container.BindInterfacesAndSelfTo<AudioRepository>().FromInstance(audioRepository).AsSingle().NonLazy();
        Container.BindInterfacesTo<AudioManager>().AsSingle();
        Container.BindMemoryPool<AudioSource, AudioSourcePool>()
            .WithMaxSize(16)
            .FromComponentInNewPrefab(audioSourcePrefab)
            .AsSingle();

        #endregion
    }
}
