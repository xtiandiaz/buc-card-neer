using UnityEngine;
using Zenject;

public class Installer : MonoInstaller
{
    [SerializeField] private BoardCamera camera;
    [SerializeField] private GamePalette palette;
    [SerializeField] private BoardView boardView;
    [SerializeField] private CardPlayer playerCard;

    [Header("Settings")] 
    [SerializeField] private CardAnimationSettings cardAnimationSettings;
    [SerializeField] private BoardLayoutSettings boardLayoutSettings;

    public override void InstallBindings()
    {
        // Main factories
        Container.Bind<IBoardFactory>().To<BoardFactory>().AsSingle();
        Container.Bind<IShipFactory>().To<ShipFactory>().AsSingle();
        Container.Bind<ISeaFactory>().To<SeaFactory>().AsSingle();
        Container.Bind<IDeckFactory>().To<DeckFactory>().AsSingle();
        Container.Bind<ICardFactory>().To<CardFactory>().AsSingle();
        Container.Bind<ISlotFactory>().To<SlotFactory>().AsSingle();
        
        //Sub-factories
        Container.BindFactory<ISea, IShip[], IDeck[], Board, Board.Factory>().AsSingle();
        Container.BindFactory<IBoard, IBoardView, BoardController, BoardController.Factory>().AsSingle();
        /*Container.BindFactory<IOcean, IShip[], IDeck[], Board, Board.Factory, IBoardFactory>()
            .AsSingle();
        Container.BindFactoryCustomInterface<IBoard, IBoardView, BoardController, BoardController.Factory, IBoardControllerFactory>()
            .AsSingle();*/
        
        Container.BindFactory<ISlot[], ShipPlayer, ShipPlayer.Factory>().AsSingle();
        Container.BindFactory<ISlot[], ShipMerchant, ShipMerchant.Factory>().AsSingle();
        Container.BindFactory<ISlot[], ShipPirate, ShipPirate.Factory>().AsSingle();
        
        Container.BindFactory<IShip, IShipView, ShipController, ShipController.Factory>().AsSingle();
        Container.BindFactory<IShipPlayer, ShipPlayerView, ICardPlayer, ShipPlayerController, ShipPlayerController.Factory>()
            .AsSingle();

        Container.BindFactory<IDeck, DeckController, DeckController.Factory>().AsSingle();

        Container.BindFactory<ISlot[], Sea, Sea.Factory>().AsSingle();
        Container.BindFactory<ISea, ISeaView, SeaController, SeaController.Factory>().AsSingle();
        
        // Slot
        Container.BindFactory<IPile, SlotBoarding, SlotBoarding.Factory>();
        Container.BindFactory<IPile, SlotEvent, SlotEvent.Factory>();
        Container.BindFactory<IPile, SlotPlayer, SlotPlayer.Factory>();
        Container.BindFactory<ResourceType, IPile, SlotStorage, SlotStorage.Factory>();
        Container.BindFactory<ISlot, ISlotView, SlotController, SlotController.Factory>().AsSingle();
        
        // Pile
        Container.BindFactory<ICardArrangement, uint?, Pile, Pile.Factory>();

        // Card
        Container.BindFactory<CardPirate, CardPirateView, CardPirateController, CardPirateController.Factory>().AsSingle();
        Container.BindFactory<CardMerchant, CardMerchantView, CardMerchantController, CardMerchantController.Factory>().AsSingle();
        Container.BindFactory<CardResource, CardResourceView, CardResourceController, CardResourceController.Factory>().AsSingle();
        Container.BindFactory<CardPlayer, CardPlayerView, CardPlayerController, CardPlayerController.Factory>().AsSingle();
        Container.BindFactory<string, CardView, CardView.Factory>().FromFactory<PrefabResourceFactory<CardView>>();
        
        // Board 
        Container.BindInterfacesAndSelfTo<BoardCamera>().FromInstance(camera).AsSingle();
        Container.Bind<Viewport>().FromMethod(() => camera.GetViewport(0)).AsSingle();
        Container.Bind<BoardLayoutSettings>().FromInstance(boardLayoutSettings).AsSingle();
        Container.BindInterfacesAndSelfTo<BoardView>().FromInstance(boardView).AsSingle();
        
        // Single Cards & Extras
        Container.Bind<CardAnimationSettings>().FromInstance(cardAnimationSettings);
        Container.Bind(typeof(ICardPlayer), typeof(IPlayerStats)).FromInstance(playerCard.Clone()).AsSingle();
        
        // Managers
        Container.BindInterfacesAndSelfTo<DealingManager>().AsSingle();
        
        // Game
        Container.Bind<GameState>().AsSingle();
        Container.Bind<GamePalette>().FromInstance(palette);
        Container.BindInterfacesAndSelfTo<GameController>().AsSingle();
    }
}
