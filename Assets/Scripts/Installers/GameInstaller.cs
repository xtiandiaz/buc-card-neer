using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [Header("Prefabs")] 
    [SerializeField] private BoardView boardView;

    [Header("Data Models")] 
    [SerializeField] private BoardModel board;
    [Space]
    [SerializeField] private DeckModel deck;
    [Space] 
    [SerializeField] private PlayerCardModel player;
    [Space]
    [SerializeField] private List<SlotModel> supplySlots;
    [Space]
    [SerializeField] private SlotModel plank;
    [SerializeField] private SlotModel helm;
    [SerializeField] private SlotModel storage;
    [SerializeField] private SlotModel mount;

    [Header("Viewport")]
    [SerializeField] private new GameCamera camera;
    
    [Header("Views")]
    [SerializeField] private GameMenuView gameMenuView;

    [Header("Decks")] 
    [SerializeField] private Deck mainDeck;

    [Header("Settings")] 
    [SerializeField] private CardAnimationSettings cardAnimationSettings;

    [Header("Audio")] 
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private AudioSource cardAudioSourcePrefab;

    public override void InstallBindings()
    {
        #region Viewport

        Container.BindInterfacesAndSelfTo<GameCamera>().FromInstance(camera).AsSingle();
        camera.Initialize(board);

        Container.Bind<Viewport>().FromMethod(() => camera.GetViewport(0));

        #endregion

        #region Controllers

        Container.BindInterfacesAndSelfTo<CardRouter>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<CardDealer>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<CardDeferrer>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<CardMatcher>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<CardHost>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<CardClasher>().AsSingle().NonLazy();
        
        Container.BindInterfacesAndSelfTo<GameController>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<BoardingController>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<ClashingController>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<SupplyController>().AsSingle().NonLazy();

        #endregion
        
        #region UI
        
        Container.Bind<IGameMenuView>().FromInstance(gameMenuView).AsSingle();

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
        Container.BindInterfacesAndSelfTo<SlotFactory>().AsSingle();
        
        #endregion
        
        #region Cards
        
        Container.BindFactory<ICardModel, ICardView, AudioSource, Card, Card.Factory>().AsSingle();
        Container.BindFactory<IPlayerCardModel, IPlayerCardView, AudioSource, PlayerCard, PlayerCard.Factory>().AsSingle();
        Container.BindInterfacesAndSelfTo<CardFactory>().AsSingle();
        
        Container.BindInterfacesAndSelfTo<IPlayerCard>()
            .FromResolveGetter<ICardFactory, IPlayerCard>(x => (IPlayerCard) x.Create(player)).AsSingle();

        #endregion
        
        #region Settings

        Container.Bind<CardAnimationSettings>().FromInstance(cardAnimationSettings).AsSingle();

        #endregion

        #region Audio

        Container.BindInterfacesAndSelfTo<AudioManager>().FromInstance(audioManager).AsSingle().NonLazy();
        Container.Bind<AudioSource>().WithId("Card Audio Source").FromInstance(cardAudioSourcePrefab)
            .WhenInjectedInto<CardFactory>();

        #endregion
    }
}
