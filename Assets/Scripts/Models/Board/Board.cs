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
    IEnumerable<IShip> Ships { get; }
    IEnumerable<IDeck> Decks { get; }

    void Initialize();
}

public class Board : IBoard
{
    public class Factory : PlaceholderFactory<IOcean, IEnumerable<IShip>, IEnumerable<IDeck>, Board>
    {
    }
    
    private readonly ReactiveProperty<BoardMode> mode = new ReactiveProperty<BoardMode>(BoardMode.Seafaring);

    private readonly IDeck eventDeck;
    
    private Board(
        IOcean ocean,
        IEnumerable<IShip> ships,
        IEnumerable<IDeck> decks
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
    public IEnumerable<IShip> Ships { get; }
    public IEnumerable<IDeck> Decks { get; }

    public void Initialize()
    {
        Ocean.Populate(eventDeck);
    }
}