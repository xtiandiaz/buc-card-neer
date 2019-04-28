using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Installer : MonoInstaller
{
    [SerializeField] private BoardCamera camera;
    [SerializeField] private GamePalette palette;

    [Header("Board")] 
    [SerializeField] private BoardView boardView;

    public override void InstallBindings()
    {
        // Main factories
        Container.Bind<BoardFactory>().AsSingle();
        Container.Bind<ShipFactory>().AsSingle();
        Container.Bind<OceanFactory>().AsSingle();
        Container.Bind<DeckFactory>().AsSingle();
        Container.Bind<CardFactory>().AsSingle();
        Container.Bind<SlotFactory>().AsSingle();
        
        //Sub-factories
        Container.BindFactory<IOcean, IShip[], IDeck[], Board, Board.Factory>().AsSingle();
        Container.BindFactory<IBoard, IBoardView, BoardController, BoardController.Factory>().AsSingle();
        
        Container.BindFactory<ISlot[], ShipPlayer, ShipPlayer.Factory>().AsSingle();
        Container.BindFactory<IShip, IShipView, ShipController, ShipController.Factory>().AsSingle();

        Container.BindFactory<IDeck, DeckController, DeckController.Factory>().AsSingle();

        Container.BindFactory<ISlot[], Ocean, Ocean.Factory>().AsSingle();
        Container.BindFactory<IOcean, IOceanView, OceanController, OceanController.Factory>().AsSingle();
        
        // Deck
        Container.BindFactory<uint, SlotBoarding, SlotBoarding.Factory>();
        Container.BindFactory<uint, SlotDefense, SlotDefense.Factory>();
        Container.BindFactory<uint, SlotEvent, SlotEvent.Factory>();
        Container.BindFactory<uint, SlotPlayer, SlotPlayer.Factory>();
        Container.BindFactory<uint, SlotResource, SlotResource.Factory>();
        Container.BindFactory<ISlot, ISlotView, SlotController, SlotController.Factory>().AsSingle();

        Container.BindFactory<ICard, ICardView, CardController, CardController.Factory>().AsSingle();
        Container.BindFactory<string, CardView, CardView.Factory>().FromFactory<PrefabResourceFactory<CardView>>();
        
        // Board 
        Container.BindInterfacesAndSelfTo<BoardCamera>().FromInstance(camera).AsSingle();
        Container.BindInterfacesAndSelfTo<BoardView>().FromInstance(boardView).AsSingle();
        
        // Game
        Container.Bind<GameSettings>().AsSingle();
        Container.Bind<GameState>().AsSingle();
        Container.Bind<GamePalette>().FromInstance(palette);
        Container.BindInterfacesAndSelfTo<GameController>().AsSingle();
    }
}
