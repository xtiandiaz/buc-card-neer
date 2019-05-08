using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

public enum DeckType
{
    Events,
    Resources
}

public interface IDeck
{
    DeckType Type { get; }
    string Name { get; }
    
    IObservable<ICard> WhenSupplied { get; }

    ICard Supply();
    void TakeBack(ICard card);
    IDeck Clone();
}

[CreateAssetMenu(fileName = "Deck", menuName = "Game/Deck", order = 1)]
public class Deck : ScriptableObject, IDeck
{
    private readonly Subject<ICard> supplying = new Subject<ICard>();
    
    [SerializeField] private DeckType type;
    [SerializeField] private List<Card> referenceCards;
    private Queue<ICard> queue;

    public DeckType Type => type;
    public string Name => name;
    
    public IObservable<ICard> WhenSupplied => supplying;

    [Inject]
    private void Initialize()
    {
        var cards = referenceCards.Select(refCard => refCard.Clone()).ToList();
        
        cards.Shuffle();
        
        queue = new Queue<ICard>(cards);
    }

    public ICard Supply()
    {
        var card = queue.Dequeue();
        
        if (card != null)
            supplying.OnNext(card);

        return card;
    }
    
    public void TakeBack(ICard card)
    {
        queue.Enqueue(card);
    }
    
    public IDeck Clone()
    {
        return Instantiate(this);
    }
}