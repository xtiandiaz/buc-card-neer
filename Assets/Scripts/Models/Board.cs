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

public class Board
{
    public class Factory : PlaceholderFactory<Board>
    {
    }
    
    private readonly ReactiveProperty<BoardMode> mode = new ReactiveProperty<BoardMode>(BoardMode.Seafaring);
    
    private Board(Deck deck)
    {
        Deck = deck;
        Deck.Initialize();
    }
    
    public BoardMode Mode
    {
        get => mode.Value;
        set => mode.Value = value;
    }

    public IObservable<BoardMode> ObservableMode => mode;

    public Deck Deck { get; }
}