using System;
using System.Collections.Generic;
using System.Linq;

public class CardQueue : ICardHeap
{
    private readonly Queue<ICard> contents;

    public CardQueue(uint extent)
    {
        Count = (int) extent;
        contents = new Queue<ICard>(Count);
    }

    public int Count { get; }
    public bool HasRoom => contents.Count < Count;

    public int? Insert(ICard card)
    {
        if (contents.Count >= Count)
            return null;
        
        contents.Enqueue(card);

        Index();

        return contents.Count - 1;
    }

    public ICard Peek()
    {
        return contents.Count == 0 ? null : contents.Peek();
    }

    public ICard Pop()
    {
        var poppedItem = contents.Dequeue();
        
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