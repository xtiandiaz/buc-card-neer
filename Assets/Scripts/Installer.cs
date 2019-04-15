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
        Container.BindFactory<string, IPlayerCard, PlayerCardView, PlayerCardView.Factory>()
            .FromFactory<PrefabResourceFactory<IPlayerCard, PlayerCardView>>();
        
        Container.BindFactory<CardType, ResourceCard, ResourceCard.Factory>();
        Container.BindFactory<string, IResourceCard, ResourceCardView, ResourceCardView.Factory>()
            .FromFactory<PrefabResourceFactory<IResourceCard, ResourceCardView>>();
        
        Container.BindFactory<BaddieCard, BaddieCard.Factory>();
        Container.BindFactory<string, IBaddieCard, BaddieCardView, BaddieCardView.Factory>()
            .FromFactory<PrefabResourceFactory<IBaddieCard, BaddieCardView>>();
        
        Container.BindFactory<AbilityType, int, AbilityCard, AbilityCard.Factory>();
        Container.BindFactory<string, IAbilityCard, AbilityCardView, AbilityCardView.Factory>()
            .FromFactory<PrefabResourceFactory<IAbilityCard, AbilityCardView>>();
        
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
