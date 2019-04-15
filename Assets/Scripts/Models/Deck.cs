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
    private readonly CardFactory cardFactory;
    private readonly List<ICard> cards = new List<ICard>();

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
        
        Shuffle();
    }

    public void Queue(ICard card)
    {
        cards.Add(card);
    }

    public ICard Dequeue()
    {
        var first = cards.FirstOrDefault();
        if (first != null)
            cards.RemoveAt(0);

        return first;
    }

    private void Shuffle()
    {
        cards.Shuffle();
    }

    private void ProduceClass(CardClass cardClass)
    {
        for (var i = 0; i < cardClass.count; i++)
        {
            Queue(cardFactory.CreateModel(cardClass.type, i));
        }
    }
}