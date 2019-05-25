using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{    
    [Header("Models")]
    [SerializeField] private PlayerCard playerCard;
    
    [Header("Viewport")]
    [SerializeField] private GameCamera camera;
    
    [Header("Views")]
    [SerializeField] private GameMenuView gameMenuView;
    [SerializeField] private BoardView boardView;
    [SerializeField] private SeaView seaView;
    [SerializeField] private ShipView shipView;

    [Header("Decks")] 
    [SerializeField] private Deck mainDeck;

    [Header("Settings")] 
    [SerializeField] private CardAnimationSettings cardAnimationSettings;
    [SerializeField] private BoardLayoutSettings boardLayoutSettings;

    public override void InstallBindings()
    {
        #region Viewport

        Container.BindInterfacesAndSelfTo<GameCamera>().FromInstance(camera).AsSingle();
        camera.Initialize(boardLayoutSettings);

        Container.Bind<Viewport>().FromMethod(() => camera.GetViewport(0));

        #endregion
        
        #region Game

        Container.BindInterfacesAndSelfTo<Game>().AsSingle();
        Container.BindInterfacesAndSelfTo<GameController>().AsSingle();
        Container.Bind<IGameMenuView>().FromInstance(gameMenuView).AsSingle();

        #endregion
        
        #region Board

        Container.Bind<BoardLayoutSettings>().FromInstance(boardLayoutSettings).AsSingle();
        Container.BindFactory<Board, Board.Factory>().AsSingle();
        Container.BindFactory<BoardView, BoardView.Factory>().FromInstance(boardView);
        Container.BindFactory<IBoard, IBoardView, BoardController, BoardController.Factory>().AsSingle();
        //Container.BindInterfacesAndSelfTo<BoardFactory>().AsSingle();
        Container.Bind<IBoard>().FromFactory<BoardFactory>().AsSingle().NonLazy();

        Container.Bind(typeof(IMoveObservable), typeof(IMoveObserver)).To<MoveRouter>().AsSingle();

        #endregion
        
        #region Sea

        Container.BindFactory<ISlot[], Sea, Sea.Factory>().AsSingle();
        Container.BindFactory<ISeaView, SeaView.Factory>().FromInstance(seaView);
        Container.BindFactory<ISea, ISeaView, SeaController, SeaController.Factory>().AsSingle();
        //Container.BindInterfacesAndSelfTo<SeaFactory>().AsSingle();
        Container.Bind<ISea>().FromFactory<SeaFactory>().AsSingle().NonLazy();

        #endregion
        
        #region Ships

        Container.BindFactory<ISlot[], Ship, Ship.Factory>().AsSingle();
        Container.BindFactory<ShipView, ShipView.Factory>().FromInstance(shipView);
        Container.BindFactory<IShip, IShipView, ShipController, ShipController.Factory>().AsSingle();
        Container.BindInterfacesAndSelfTo<ShipFactory>().AsSingle();

        #endregion
        
        #region Decks & Providers
        
        Container.Bind(typeof(ICardProvider), typeof(IDeck))
            //.WithId(DeckType.Events)
            .FromInstance(Instantiate(mainDeck));
        
        Container.BindFactory<IDeck, DeckController, DeckController.Factory>().WhenInjectedInto<IDeckFactory>();
        Container.BindInterfacesAndSelfTo<DeckFactory>().AsSingle();
        
        #endregion
        
        #region Slots
        
        Container.BindFactory<IPile, ISlotSettings, Bounds, Transform, BoardingSlot, BoardingSlot.Factory>();
        Container.BindFactory<IPile, ISlotSettings, Bounds, Transform, SupplySlot, SupplySlot.Factory>();
        Container.BindFactory<IPile, ISlotSettings, Bounds, Transform, PlayerSlot, PlayerSlot.Factory>();
        Container.BindFactory<IPile, ISlotSettings, Bounds, Transform, StorageSlot, StorageSlot.Factory>();
        Container.BindFactory<ISlot, ISlotView, SlotController, SlotController.Factory>().AsSingle();
        Container.BindInterfacesAndSelfTo<SlotFactory>().AsSingle();
        
        #endregion
        
        #region Cards

        Container.BindFactory<PirateCard, PirateCardView, PirateCardController, PirateCardController.Factory>().AsSingle();
        Container.BindFactory<IMerchantCard, IMerchantCardView, MerchantCardController, MerchantCardController.Factory>().AsSingle();
        Container.BindFactory<IInspectorCard, IInspectorCardView, InspectorCardController, InspectorCardController.Factory>().AsSingle();
        Container.BindFactory<ResourceCard, ResourceCardView, ResourceCardController, ResourceCardController.Factory>().AsSingle();
        Container.BindFactory<PlayerCard, PlayerCardView, PlayerCardController, PlayerCardController.Factory>().AsSingle();
        Container.BindFactory<string, CardView, CardView.Factory>().FromFactory<PrefabResourceFactory<CardView>>();
        Container.BindInterfacesAndSelfTo<CardFactory>().AsSingle();
        
        Container.BindInterfacesAndSelfTo<PlayerCard>().FromInstance(Instantiate(playerCard));

        #endregion
        
        #region Settings

        Container.Bind<CardAnimationSettings>().FromInstance(cardAnimationSettings).AsSingle();

        #endregion
    }
}
