using UnityEngine;
using Zenject;

public class Installer : MonoInstaller
{
    [SerializeField] private BoardCamera camera;
    [SerializeField] private BoardView boardView;
    [SerializeField] private CardTileView cardTileViewPrefab;
    [SerializeField] private GamePalette palette;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<BoardCamera>().FromInstance(camera).AsSingle();
        
        Container.BindFactory<Coordinates, CardTile, CardTile.Factory>();
        Container.BindFactory<ICardTile, CardTileView, CardTileView.Factory>().FromComponentInNewPrefab(cardTileViewPrefab);
        Container.Bind<CardTileFactory>().AsSingle();

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
        
        Container.Bind<CardFactory>().AsSingle();

        Container.BindFactory<DeckContents, Deck, Deck.Factory>();

        Container.BindFactory<int, int, Board, Board.Factory>();
        Container.BindInterfacesAndSelfTo<BoardController>().AsSingle();
        Container.Bind<BoardView>().FromInstance(boardView).AsSingle();
        
        Container.Bind<GameSettings>().AsSingle();
        Container.Bind<GameState>().AsSingle();
        Container.Bind<GamePalette>().FromInstance(palette);
        Container.BindInterfacesAndSelfTo<GameController>().AsSingle();
    }
}
