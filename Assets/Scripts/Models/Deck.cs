using System.Collections.Generic;
using System.Linq;
using Zenject;

public struct CardClass
{
    public CardType type;
    public int count;

    public CardClass(CardType type, int count)
    {
        this.type = type;
        this.count = count;
    }
}

public struct DeckContents
{
    public CardClass[] classes;
    public int count;

    public DeckContents(params CardClass[] classes)
    {
        this.classes = classes;
        count = classes.Sum(c => c.count);
    }
}

public class Deck
{
    private CardFactory cardFactory;
    private List<Card> cards = new List<Card>();

    public class Factory : PlaceholderFactory<DeckContents, Deck>
    {
    }

    private Deck(
        DeckContents contents, 
        CardFactory cardFactory)
    {
        this.cardFactory = cardFactory;
        
        foreach (var cardClass in contents.classes)
        {
            ProduceClass(cardClass);
        }

        cards.Shuffle();
    }

    public void Push(Card card)
    {
        cards.Add(card);
    }

    public Card Pull()
    {
        var first = cards.FirstOrDefault();
        if (first != null)
            cards.RemoveAt(0);

        return first;
    }

    private void ProduceClass(CardClass cardClass)
    {
        for (var i = 0; i < cardClass.count; i++)
        {
            Push(cardFactory.CreateModel(cardClass.type, i));
        }
    }
}