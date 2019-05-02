using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

public enum BoardMode
{
    Seafaring,
    Trade,
    Combat
}

public interface IBoard
{
    ISea Sea { get; }
    IShip[] Ships { get; }
    IDeck[] Decks { get; }
    BoardMode Mode { get; set; }
    ISlot[] PlaySlots { get; }
    
    IObservable<ICard> CardPicked { get; }
    IObservable<(ICard, Vector3)> CardDropped { get; }
    IObservable<BoardMode> ModeChanged { get; }

    void Deal();
}

public class Board : IBoard
{
    public class Factory : PlaceholderFactory<ISea, IShip[], IDeck[], Board>
    {
    }
    
    private readonly ReactiveProperty<BoardMode> mode = new ReactiveProperty<BoardMode>(BoardMode.Seafaring);
    private readonly IDeck eventDeck;
    
    private Board(ISea sea, IShip[] ships, IDeck[] decks)
    {
        Sea = sea;
        Ships = ships;
        Decks = decks;

        eventDeck = Decks.FirstOrDefault(d => d.Type == DeckType.Events);
        
        ShipPlayer = (ShipPlayer) Ships.First(s => s.Type == ShipType.Player);
        ShipMerchant = (ShipMerchant) Ships.First(s => s.Type == ShipType.Merchant);
        ShipPirate = (ShipPirate) Ships.First(s => s.Type == ShipType.Pirate);
        
        var playSlots = ShipPlayer.Slots.ToList();
        playSlots.AddRange(Sea.Slots);
        
        PlaySlots = playSlots.ToArray();
    }
    
    public BoardMode Mode
    {
        get => mode.Value;
        set => mode.Value = value;
    }

    public IObservable<BoardMode> ModeChanged => mode;

    public ISea Sea { get; }
    public IShip[] Ships { get; }
    public IDeck[] Decks { get; }
    public ISlot[] PlaySlots { get; private set; }

    public IObservable<ICard> CardPicked => 
        Decks.Select(d => d.Supplied).Merge().SelectMany(c => c.Picked.Select(_ => c));
    
    public IObservable<(ICard, Vector3)> CardDropped =>
        Decks.Select(d => d.Supplied).Merge().SelectMany(c => c.Dropped.Select(dropPosition => (c, dropPosition)));
    
    public ShipPlayer ShipPlayer { get; private set; }
    public ShipMerchant ShipMerchant { get; private set; }
    public ShipPirate ShipPirate { get; private set; }

    public void Deal()
    {
        Sea.Populate(eventDeck);
    }
}