using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public enum DeckType
{
    Events,
    Resources
}

public interface IDeck
{
    DeckType Type { get; }
    IObservable<ICard> Supplied { get; }

    void Initialize();
    ICard Supply(bool facingDown);
    void TakeBack(ICard card);
}

[CreateAssetMenu(fileName = "Deck", menuName = "Game/Deck", order = 1)]
public class Deck : ScriptableObject, IDeck
{
    private readonly Subject<ICard> supplied = new Subject<ICard>();
    
    [SerializeField] private DeckType type;
    [SerializeField] private List<Card> cards;
    
    private Queue<ICard> queue;

    public DeckType Type => type;
    public IObservable<ICard> Supplied => supplied;

    public void Initialize()
    {
        var playCards = cards.Select(card => card.Clone()).ToList();
        
        playCards.Shuffle();
        
        queue = new Queue<ICard>(playCards);
    }

    public ICard Supply(bool facingDown)
    {
        var card = queue.Dequeue();
        if (card == null)
            return null;
        
        card.Flip(facingDown ? CardFace.Back : CardFace.Front);
        
        supplied.OnNext(card);

        return card;
    }
    
    public void TakeBack(ICard card)
    {
        queue.Enqueue(card);
    }
}