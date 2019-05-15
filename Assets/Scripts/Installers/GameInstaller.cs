using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{    
    [Header("Models")]
    [SerializeField] private CardPlayer playerCard;
    
    [Header("Viewport")]
    [SerializeField] private GameCamera camera;
    
    [Header("Views")]
    [SerializeField] private GameMenuView gameMenuView;
    [SerializeField] private BoardView boardView;
    [SerializeField] private SeaView seaView;
    [SerializeField] private ShipView shipView;

    [Header("Decks")] 
    [SerializeField] private Deck eventsDeck;

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
        Container.BindInterfacesAndSelfTo<BoardFactory>().AsSingle();

        #endregion
        
        #region Sea

        Container.BindFactory<ISlot[], Sea, Sea.Factory>().AsSingle();
        Container.BindFactory<ISeaView, SeaView.Factory>().FromInstance(seaView);
        Container.BindFactory<ISea, ISeaView, SeaController, SeaController.Factory>().AsSingle();
        Container.BindInterfacesAndSelfTo<SeaFactory>().AsSingle();

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
            .FromInstance(Instantiate(eventsDeck));
        
        Container.BindFactory<IDeck, DeckController, DeckController.Factory>().WhenInjectedInto<IDeckFactory>();
        Container.BindInterfacesAndSelfTo<DeckFactory>().AsSingle();
        
        #endregion
        
        #region Slots
        
        Container.BindFactory<IPile, Transform, Bounds, SlotBoarding, SlotBoarding.Factory>();
        Container.BindFactory<IPile, Transform, Bounds,SlotEvent, SlotEvent.Factory>();
        Container.BindFactory<IPile, Transform, Bounds, SlotPlayer, SlotPlayer.Factory>();
        Container.BindFactory<ResourceType, IPile, Transform, Bounds, SlotStorage, SlotStorage.Factory>();
        Container.BindFactory<ISlot, ISlotView, SlotController, SlotController.Factory>().AsSingle();
        Container.BindInterfacesAndSelfTo<SlotFactory>().AsSingle();
        
        Container.BindFactory<ICardArrangement, int?, Pile, Pile.Factory>();
        
        #endregion
        
        #region Cards

        Container.BindFactory<CardPirate, CardPirateView, CardPirateController, CardPirateController.Factory>().AsSingle();
        Container.BindFactory<CardMerchant, CardMerchantView, CardMerchantController, CardMerchantController.Factory>().AsSingle();
        Container.BindFactory<CardResource, CardResourceView, CardResourceController, CardResourceController.Factory>().AsSingle();
        Container.BindFactory<CardPlayer, CardPlayerView, CardPlayerController, CardPlayerController.Factory>().AsSingle();
        Container.BindFactory<string, CardView, CardView.Factory>().FromFactory<PrefabResourceFactory<CardView>>();
        Container.BindInterfacesAndSelfTo<CardFactory>().AsSingle();
        
        Container.Bind(typeof(ICardPlayer), typeof(IPlayerStats)).FromInstance(Instantiate(playerCard));

        #endregion
        
        #region Settings

        Container.Bind<CardAnimationSettings>().FromInstance(cardAnimationSettings).AsSingle();

        #endregion
    }
}
