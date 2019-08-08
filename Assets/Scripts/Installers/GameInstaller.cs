using System.Collections.Generic;
using UnityEngine;

public class GameInstaller : SceneInstaller
{
    [Header("Data Models")]
    [SerializeField] private DeckModel deck = default;
    [Space] 
    [SerializeField] private PlayerCardModel player = default;
    [Space]
    [SerializeField] private List<SlotModel> supplySlots = default;
    [Space]
    [SerializeField] private SlotModel plank = default;
    [SerializeField] private SlotModel helm = default;
    [SerializeField] private SlotModel storage = default;
    [SerializeField] private SlotModel mount = default;
    
    [Header("Views")] 
    [SerializeField] private BoardView boardViewPrefab = default;

    [Header("Menus")]
    [SerializeField] private GameMenu gameMenuPrefab = default;

    public override void InstallBindings()
    {
        base.InstallBindings();
        
        #region Controllers

        Container.BindInterfacesTo<CardRouter>().AsSingle().NonLazy();
        Container.BindInterfacesTo<CardDealer>().AsSingle().NonLazy();
        Container.BindInterfacesTo<CardDeferrer>().AsSingle().NonLazy();
        Container.BindInterfacesTo<CardForwarder>().AsSingle().NonLazy();
        Container.BindInterfacesTo<CardDismisser>().AsSingle().NonLazy();
        Container.BindInterfacesTo<CardMatcher>().AsSingle().NonLazy();
        Container.BindInterfacesTo<CardHost>().AsSingle().NonLazy();
        Container.BindInterfacesTo<CardClasher>().AsSingle().NonLazy();
        Container.BindInterfacesTo<CardShooter>().AsSingle().NonLazy();

        Container.BindInterfacesTo<GameController>().AsSingle().NonLazy();
        Container.BindInterfacesTo<BoardController>().AsSingle().NonLazy();
        Container.BindInterfacesTo<BoardingController>().AsSingle().NonLazy();
        Container.BindInterfacesTo<ClashingController>().AsSingle().NonLazy();
        Container.BindInterfacesTo<SupplyController>().AsSingle().NonLazy();
        Container.BindInterfacesTo<CombatController>().AsSingle().NonLazy();
        Container.BindInterfacesTo<VisualEffectsController>().AsSingle().NonLazy();

        #endregion
        
        #region UI
        
        Container.BindInterfacesTo<GameMenu>().FromComponentInNewPrefab(gameMenuPrefab).AsSingle().NonLazy();

        #endregion
        
        #region Board
        
        Container.Bind<IBoardView>().FromComponentInNewPrefab(boardViewPrefab).AsSingle();

        #endregion

        #region Sea

        Container.BindFactory<IEnumerable<ISlot>, ISeaView, Sea, Sea.Factory>().AsSingle();
        Container.Bind<List<SlotModel>>().FromInstance(supplySlots).WhenInjectedInto<SeaFactory>();
        Container.Bind<ISea>().FromFactory<SeaFactory>().AsSingle();

        #endregion
        
        #region Ship

        Container.BindFactory<ISlot, ISlot, ISlot, ISlot, IShipView, Ship, Ship.Factory>()
            .AsSingle();
        Container.Bind<ISlotModel>().FromInstance(helm).WhenInjectedInto<ShipFactory>();
        Container.Bind<ISlotModel>().FromInstance(plank).WhenInjectedInto<ShipFactory>();
        Container.Bind<ISlotModel>().FromInstance(storage).WhenInjectedInto<ShipFactory>();
        Container.Bind<ISlotModel>().FromInstance(mount).WhenInjectedInto<ShipFactory>();
        Container.Bind<IShip>().FromFactory<ShipFactory>().AsSingle();

        #endregion
        
        #region Deck & Card Providers
        
        Container.BindFactory<List<ICardModel>, Deck, Deck.Factory>().AsSingle();
        Container.Bind<IDeckFactory>().To<DeckFactory>().AsSingle();
        Container.Bind(typeof(IDeck))
            .FromResolveGetter<IDeckFactory, IDeck>(x => x.Create(deck)).AsSingle();
        
        #endregion
        
        #region Slots

        Container.BindFactory<ISlotModel, ISlotView, Slot, Slot.Factory>().AsSingle();
        Container.BindFactory<ISlotModel, IStashSlotView, StashSlot, StashSlot.Factory>().AsSingle();
        Container.BindInterfacesTo<SlotFactory>().AsSingle();
        
        #endregion
        
        #region Cards
        
        Container.BindFactory<ICardModel, ICardView, Card, Card.Factory>().AsSingle();
        Container.BindFactory<IPlayerCardModel, IPlayerCardView, PlayerCard, PlayerCard.Factory>().AsSingle();
        Container.BindInterfacesTo<CardFactory>().AsSingle();

        Container.BindInterfacesAndSelfTo<IPlayerCard>()
            .FromResolveGetter<ICardFactory, IPlayerCard>(x => (IPlayerCard) x.Create(player)).AsSingle();

        #endregion
    }
}
