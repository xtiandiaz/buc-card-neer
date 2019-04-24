using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "Deck", menuName = "Game/Deck", order = 1)]
public class Deck : ScriptableObject, IInitializable
{
    [SerializeField] private List<Card> cards;
    
    private Queue<ICard> cardQueue;

    public void Initialize()
    {
        cards.Shuffle();
        
        cardQueue = new Queue<ICard>(cards);
    }

    public ICard Supply()
    {
        return cardQueue.Dequeue();
    }
    
    public void PutBack(ICard card)
    {
        cardQueue.Enqueue(card);
    }
}