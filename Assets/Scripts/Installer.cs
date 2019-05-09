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
        Container.BindInterfacesAndSelfTo<ShipFactory>().AsSingle();
        Container.BindInterfacesAndSelfTo<SeaFactory>().AsSingle();
        Container.BindInterfacesAndSelfTo<DeckFactory>().AsSingle();
        Container.BindInterfacesAndSelfTo<CardFactory>().AsSingle();
        Container.BindInterfacesAndSelfTo<SlotFactory>().AsSingle();
        
        // Providers
        Container.Bind<IPlayerProvider>().To<PlayerProvider>().AsSingle();
        
        // Player
        Container.Bind(typeof(ICardPlayer), typeof(IPlayerStats)).FromInstance(playerCard.Clone()).AsSingle();
        
        // Board 
        Container.BindFactory<ISea, IShip[], IDeck[], Board, Board.Factory>().AsSingle();
        Container.BindFactory<IBoard, IBoardView, BoardController, BoardController.Factory>().AsSingle();
        
        Container.Bind<BoardLayoutSettings>().FromInstance(boardLayoutSettings).AsSingle();
        Container.BindInterfacesAndSelfTo<BoardView>().FromInstance(boardView).AsSingle();
        Container.Bind<IBoard>().FromFactory<BoardFactory>().AsSingle();
        
        Container.BindFactory<ISlot[], Sea, Sea.Factory>().AsSingle();
        Container.BindFactory<ISea, ISeaView, SeaController, SeaController.Factory>().AsSingle();
        
        // Ship
        Container.BindFactory<ISlot[], ShipPlayer, ShipPlayer.Factory>().AsSingle();
        Container.BindFactory<ISlot[], ShipMerchant, ShipMerchant.Factory>().AsSingle();
        Container.BindFactory<ISlot[], ShipPirate, ShipPirate.Factory>().AsSingle();
        
        Container.BindFactory<IShip, IShipView, ShipController, ShipController.Factory>().AsSingle();
        Container.BindFactory<IShipPlayer, ShipPlayerView, ShipPlayerController, ShipPlayerController.Factory>()
            .AsSingle();
        
        // Deck
        Container.BindFactory<IDeck, DeckController, DeckController.Factory>().AsSingle();
        
        // Slot
        Container.BindFactory<IPile, Transform, Bounds, SlotBoarding, SlotBoarding.Factory>();
        Container.BindFactory<IPile, Transform, Bounds,SlotEvent, SlotEvent.Factory>();
        Container.BindFactory<IPile, Transform, Bounds, SlotPlayer, SlotPlayer.Factory>();
        Container.BindFactory<ResourceType, IPile, Transform, Bounds, SlotStorage, SlotStorage.Factory>();
        Container.BindFactory<ISlot, ISlotView, SlotController, SlotController.Factory>().AsSingle();
        
        // Pile
        Container.BindFactory<ICardArrangement, int?, Pile, Pile.Factory>();

        // Card
        Container.BindFactory<CardPirate, CardPirateView, CardPirateController, CardPirateController.Factory>().AsSingle();
        Container.BindFactory<CardMerchant, CardMerchantView, CardMerchantController, CardMerchantController.Factory>().AsSingle();
        Container.BindFactory<CardResource, CardResourceView, CardResourceController, CardResourceController.Factory>().AsSingle();
        Container.BindFactory<CardPlayer, CardPlayerView, CardPlayerController, CardPlayerController.Factory>().AsSingle();
        Container.BindFactory<string, CardView, CardView.Factory>().FromFactory<PrefabResourceFactory<CardView>>();
        
        // Viewport
        Container.BindInterfacesAndSelfTo<BoardCamera>().FromInstance(camera).AsSingle();
        Container.Bind<Viewport>().FromFactory<ViewportFactory>().AsSingle();
        
        // Settings
        Container.Bind<CardAnimationSettings>().FromInstance(cardAnimationSettings);
        
        // Managers
        Container.BindInterfacesAndSelfTo<DealingManager>().AsSingle();
        
        // Game
        Container.Bind<GameState>().AsSingle();
        Container.Bind<GamePalette>().FromInstance(palette);
        Container.BindInterfacesAndSelfTo<GameController>().AsSingle();
    }
}
