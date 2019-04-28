using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Zenject;

public enum BoardMode
{
    Seafaring,
    Trade,
    Combat
}

public interface IBoard
{
    IOcean Ocean { get; }
    IShip[] Ships { get; }
    IDeck[] Decks { get; }
    IObservable<ICard> CardPicked { get; }
    IObservable<ICard> CardDropped { get; }

    IShip ShipPlayer { get; }
    ISlot[] PlaySlots { get; }

    void Initialize();
    void Deal();
}

public class Board : IBoard
{
    public class Factory : PlaceholderFactory<IOcean, IShip[], IDeck[], Board>
    {
    }
    
    private readonly ReactiveProperty<BoardMode> mode = new ReactiveProperty<BoardMode>(BoardMode.Seafaring);
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    private readonly IDeck eventDeck;
    
    private Board(
        IOcean ocean,
        IShip[] ships,
        IDeck[] decks
        )
    {
        Ocean = ocean;
        Ships = ships;
        Decks = decks;

        eventDeck = Decks.FirstOrDefault(d => d.Type == DeckType.Events);
    }
    
    public BoardMode Mode
    {
        get => mode.Value;
        set => mode.Value = value;
    }

    public IObservable<BoardMode> ObservableMode => mode;

    public IOcean Ocean { get; }
    public IShip[] Ships { get; }
    public IDeck[] Decks { get; }

    public IObservable<ICard> CardPicked => 
        Decks.Select(d => d.Supplied).Merge().SelectMany(c => c.Picked.Select(_ => c));
    public IObservable<ICard> CardDropped =>
        Decks.Select(d => d.Supplied).Merge().SelectMany(c => c.Dropped.Select(_ => c));
    
    public IShip ShipPlayer { get; private set; }
    public ISlot[] PlaySlots { get; private set; }

    public void Initialize()
    {
        ShipPlayer = Ships.First(s => s.Type == ShipType.Player);
        PlaySlots = ShipPlayer.Slots;
    }

    public void Deal()
    {
        Ocean.Populate(eventDeck);
    }
}