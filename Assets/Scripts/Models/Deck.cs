using System.Collections.Generic;
using System.Linq;
using Zenject;

public struct CardSet
{
    public CardType type;
    public int count;

    public CardSet(CardType type, int count)
    {
        this.type = type;
        this.count = count;
    }
}

public struct DeckContents
{
    public CardSet[] sets;
    public int count;

    public DeckContents(params CardSet[] sets)
    {
        this.sets = sets;
        count = sets.Sum(c => c.count);
    }
}

public class Deck
{
    private readonly Queue<ICard> cards;

    public class Factory : PlaceholderFactory<DeckContents, Deck>
    {
    }

    private Deck(
        DeckContents contents,
        Card.Factory cardFactory
    )
    {      
        var cardList = new List<ICard>();
        foreach (var cardSet in contents.sets)
        {
            for (var i = 0; i < cardSet.count; i++)
                cardList.Add(cardFactory.Create(cardSet.type));
        }

        cardList.Shuffle();
        
        cards = new Queue<ICard>(cardList);
    }

    public ICard Supply()
    {
        return cards.Dequeue();
    }
    
    public void PutBack(ICard card)
    {
        cards.Enqueue(card);
    }
}