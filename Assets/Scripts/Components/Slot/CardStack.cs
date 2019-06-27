using System;
using System.Collections.Generic;
using System.Linq;

public class CardStack : ICardHeap
{
    private readonly Stack<ICard> contents;

    public CardStack()
    {
        contents = new Stack<ICard>();
    }

    public int Count => contents.Count;
    public bool HasRoom => true;

    public int? Insert(ICard card)
    {
        contents.Push(card);

        Index();

        return 0;
    }

    public ICard Peek()
    {
        return contents.Count == 0 ? null : contents.Peek();
    }

    public ICard Pop()
    {
        var poppedItem = contents.Pop();
        
        Index();

        return poppedItem;
    }
    
    public IEnumerable<T> Map<T>(Func<ICard, int, T> byFunction)
    {
        return contents.Select(byFunction);
    }

    public bool DoesContain(ICard card)
    {
        return contents.Contains(card);
    }

    private void Index()
    {
        var i = 0;
        foreach (var item in contents)
        {
            item.Index = i++;
        }
    }
}