using UnityEngine;
using Zenject;

public class Installer : MonoInstaller
{
    [SerializeField] private UserInteractionListener userInteractionListener;
    [SerializeField] private BoardCamera camera;
    [SerializeField] private BoardView boardView;
    [SerializeField] private CardSlotView cardSlotViewPrefab;
    [SerializeField] private GamePalette palette;

    public override void InstallBindings()
    {
        // Deck
        Container.BindFactory<DeckContents, Deck, Deck.Factory>().AsSingle();
        Container.BindFactory<Deck, DeckController, DeckController.Factory>().AsSingle();
        
        Container.BindFactory<uint, CardSlotType, uint, CardSlot, CardSlot.Factory>();

        Container.Bind<Card.Factory>().AsSingle();
        
        Container.BindFactory<PlayerCard, PlayerCard.Factory>();
        Container.BindFactory<string, PlayerCardView, PlayerCardView.Factory>()
            .FromFactory<PrefabResourceFactory<PlayerCardView>>();
        
        Container.BindFactory<ItemCard, ItemCard.Factory>();
        Container.BindFactory<string, ItemCardView, ItemCardView.Factory>()
            .FromFactory<PrefabResourceFactory<ItemCardView>>();
        
        Container.BindFactory<PirateCard, PirateCard.Factory>();
        Container.BindFactory<string, PirateCardView, PirateCardView.Factory>()
            .FromFactory<PrefabResourceFactory<PirateCardView>>();
        
        Container.BindFactory<MerchantCard, MerchantCard.Factory>();
        Container.BindFactory<string, MerchantCardView, MerchantCardView.Factory>()
            .FromFactory<PrefabResourceFactory<MerchantCardView>>();
        
        Container.BindFactory<ICardSlot, ICardSlotView, CardSlotController, CardSlotController.Factory>().AsSingle();
        Container.BindFactory<ICard, ICardView, CardController, CardController.Factory>().AsSingle();
        
        // Board
        Container.BindFactory<Board, Board.Factory>();
        Container.BindInterfacesAndSelfTo<BoardController>().AsSingle();
        Container.BindInterfacesAndSelfTo<BoardView>().FromInstance(boardView).AsSingle();
        Container.BindInterfacesAndSelfTo<BoardCamera>().FromInstance(camera).AsSingle();
        
        // UI
        Container.Bind<UserInteractionListener>().FromInstance(userInteractionListener).AsSingle();
        
        // Game
        Container.Bind<GameSettings>().AsSingle();
        Container.Bind<GameState>().AsSingle();
        Container.Bind<GamePalette>().FromInstance(palette);
        Container.BindInterfacesAndSelfTo<GameController>().AsSingle();
    }
}
