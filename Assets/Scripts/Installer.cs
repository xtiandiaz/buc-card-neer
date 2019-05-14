using UnityEngine;
using Zenject;

public class Installer : MonoInstaller
{    
    [Header("Models")]
    [SerializeField] private CardPlayer playerCard;
    
    [Header("Views")]
    [SerializeField] private BoardView boardView;
    [SerializeField] private SeaView seaView;
    [SerializeField] private ShipPlayerView shipPlayerView;

    [Header("Decks")] 
    [SerializeField] private Deck eventsDeck;
    
    [Header("Viewport")]
    [SerializeField] private BoardCamera camera;

    [Header("Settings")] 
    [SerializeField] private CardAnimationSettings cardAnimationSettings;
    [SerializeField] private BoardLayoutSettings boardLayoutSettings;

    public override void InstallBindings()
    {
        #region Game

        Container.BindInterfacesAndSelfTo<GameController>().AsSingle();

        #endregion
        
        #region Board

        Container.Bind<BoardLayoutSettings>().FromInstance(boardLayoutSettings).AsSingle();
        Container.BindFactory<Board, Board.Factory>().AsSingle();
        Container.BindFactory<BoardView, BoardView.Factory>().FromInstance(boardView);
        Container.BindFactory<IBoard, IBoardView, BoardController, BoardController.Factory>().AsSingle();
        Container.Bind<IBoardFactory>().To<BoardFactory>().AsSingle();

        #endregion
        
        #region Sea

        Container.BindFactory<ISlot[], Sea, Sea.Factory>().AsSingle();
        Container.BindFactory<ISeaView, SeaView.Factory>().FromInstance(seaView);
        Container.BindFactory<ISea, ISeaView, SeaController, SeaController.Factory>().AsSingle();
        Container.Bind<ISeaFactory>().To<SeaFactory>().AsSingle();

        #endregion
        
        #region Ships

        Container.BindFactory<ISlot[], ShipPlayer, ShipPlayer.Factory>().AsSingle();
        Container.BindFactory<IShipPlayerView, ShipPlayerView.Factory>().FromInstance(shipPlayerView);
        Container.BindFactory<IShip, IShipView, ShipController, ShipController.Factory>().AsSingle();
        Container.BindFactory<IShipPlayer, ShipPlayerView, ShipPlayerController, ShipPlayerController.Factory>()
            .AsSingle();
        Container.Bind<IShipFactory>().To<ShipFactory>().AsSingle();

        #endregion
        
        #region Decks & Providers
        
        Container.Bind(typeof(ICardProvider), typeof(IDeck))
            //.WithId(DeckType.Events)
            .FromInstance(Instantiate(eventsDeck));
        
        Container.BindFactory<IDeck, DeckController, DeckController.Factory>().WhenInjectedInto<IDeckFactory>();
        Container.Bind<IDeckFactory>().To<DeckFactory>().AsSingle();
        
        #endregion
        
        #region Slots
        
        Container.BindFactory<IPile, Transform, Bounds, SlotBoarding, SlotBoarding.Factory>();
        Container.BindFactory<IPile, Transform, Bounds,SlotEvent, SlotEvent.Factory>();
        Container.BindFactory<IPile, Transform, Bounds, SlotPlayer, SlotPlayer.Factory>();
        Container.BindFactory<ResourceType, IPile, Transform, Bounds, SlotStorage, SlotStorage.Factory>();
        Container.BindFactory<ISlot, ISlotView, SlotController, SlotController.Factory>().AsSingle();
        Container.Bind<ISlotFactory>().To<SlotFactory>().AsSingle();
        
        Container.BindFactory<ICardArrangement, int?, Pile, Pile.Factory>();
        
        #endregion
        
        #region Cards

        Container.BindFactory<CardPirate, CardPirateView, CardPirateController, CardPirateController.Factory>().AsSingle();
        Container.BindFactory<CardMerchant, CardMerchantView, CardMerchantController, CardMerchantController.Factory>().AsSingle();
        Container.BindFactory<CardResource, CardResourceView, CardResourceController, CardResourceController.Factory>().AsSingle();
        Container.BindFactory<CardPlayer, CardPlayerView, CardPlayerController, CardPlayerController.Factory>().AsSingle();
        Container.BindFactory<string, CardView, CardView.Factory>().FromFactory<PrefabResourceFactory<CardView>>();
        Container.Bind<ICardFactory>().To<CardFactory>().AsSingle();
        
        Container.Bind(typeof(ICardPlayer), typeof(IPlayerStats)).FromInstance(Instantiate(playerCard));

        #endregion
        
        #region Viewport

        Container.BindInterfacesAndSelfTo<BoardCamera>().FromInstance(camera).AsSingle();
        Container.Bind<Viewport>().FromFactory<ViewportFactory>().AsSingle();

        #endregion
        
        #region Settings

        Container.Bind<CardAnimationSettings>().FromInstance(cardAnimationSettings);

        #endregion
    }
}
