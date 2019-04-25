using UnityEngine;
using Zenject;

public class Installer : MonoInstaller
{
    [SerializeField] private BoardCamera camera;
    [SerializeField] private GamePalette palette;

    [Header("Decks")]
    [SerializeField] private Deck eventDeck;

    [Header("Board")] 
    [SerializeField] private BoardView boardView;

    public override void InstallBindings()
    {
        // Deck
        Container.Bind<Deck>().FromInstance(eventDeck);
        
        Container.BindFactory<Deck, DeckController, DeckController.Factory>().AsSingle();
        
        Container.BindFactory<CardSlotType, uint, CardSlot, CardSlot.Factory>();
        Container.BindFactory<ICardSlot, ICardSlotView, CardSlotController, CardSlotController.Factory>().AsSingle();

        Container.BindFactory<ICard, ICardView, CardController, CardController.Factory>().AsSingle();
        Container.BindFactory<string, CardView, CardView.Factory>().FromFactory<PrefabResourceFactory<CardView>>();
        
        // Board
        Container.BindFactory<Board, Board.Factory>();
        Container.BindInterfacesAndSelfTo<BoardCamera>().FromInstance(camera).AsSingle();
        Container.BindInterfacesAndSelfTo<BoardView>().FromInstance(boardView).AsSingle();
        
        Container.BindInterfacesAndSelfTo<BoardController>().AsSingle();
        
        // Game
        Container.Bind<GameSettings>().AsSingle();
        Container.Bind<GameState>().AsSingle();
        Container.Bind<GamePalette>().FromInstance(palette);
        Container.BindInterfacesAndSelfTo<GameController>().AsSingle();
    }
}
