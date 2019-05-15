using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

public enum DeckType
{
    Events
}

public interface IDeck : ICardProvider
{
    DeckType Type { get; }

    void Shuffle();
}

[CreateAssetMenu(fileName = "Deck", menuName = "Game/Deck", order = 1)]
public class Deck : ScriptableObject, IDeck
{
    public class Factory : PlaceholderFactory<IDeck>
    {
    }
    
    private readonly Subject<ICard> provision = new Subject<ICard>();
    
    [SerializeField] private DeckType type;
    [SerializeField] private List<Card> cards;
    private Queue<ICard> queue;

    public DeckType Type => type;
    
    public IObservable<ICard> WhenProvided => provision;
    public bool IsExhausted { get; private set; }

    public void Shuffle()
    {
        cards.Shuffle();
    }

    public ICard Provide()
    {
        var card = cards.LastOrDefault();

        if (card != null)
        {
            card = Instantiate(card);
            cards.RemoveAt(cards.Count - 1);
            provision.OnNext(card);
        }
        
        IsExhausted = cards.Count <= 0;

        return card;
    }
}