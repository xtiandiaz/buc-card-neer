using UnityEngine;
using Zenject;

public class Installer : MonoInstaller
{
    [SerializeField] private BoardCamera camera;
    [SerializeField] private BoardView boardView;
    [SerializeField] private CardSlotView cardSlotViewPrefab;
    [SerializeField] private GamePalette palette;

    public override void InstallBindings()
    {
        // Deck
        Container.BindFactory<DeckContents, Deck, Deck.Factory>().AsSingle();
        Container.BindFactory<Deck, DeckController, DeckController.Factory>().AsSingle();
        Container.Bind<DeckFactory>().AsSingle();
        
        Container.BindFactory<int, CardSlot, CardSlot.Factory>();
        Container.BindFactory<ICardSlot, ICardSlotView, CardSlotView.Factory>().FromComponentInNewPrefab(cardSlotViewPrefab);
        Container.Bind<CardSlotFactory>().AsSingle();

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
        Container.Bind<CardFactory>().AsSingle();
        
        // Board
        Container.BindFactory<int, int, Board, Board.Factory>();
        Container.BindInterfacesAndSelfTo<BoardController>().AsSingle();
        Container.Bind<BoardView>().FromInstance(boardView).AsSingle();
        Container.BindInterfacesAndSelfTo<BoardCamera>().FromInstance(camera).AsSingle();
        
        Container.Bind<GameSettings>().AsSingle();
        Container.Bind<GameState>().AsSingle();
        Container.Bind<GamePalette>().FromInstance(palette);
        Container.BindInterfacesAndSelfTo<GameController>().AsSingle();
    }
}
