using System;
using System.Collections.Generic;
using System.Linq;

public interface IPile
{
    int Extent { get; }
    int Count { get; }
    bool HasRoom { get; }

    int? Insert(ICard card);
    bool Remove(ICard card);
    ICard Peek();
    ICard Pop();
    IEnumerable<T> Map<T>(Func<ICard, int, T> byFunction);
    bool DoesContain(ICard card);
}

public class Pile : IPile
{
    private readonly Mode mode;
    private readonly List<ICard> contents;
    private readonly int extent;

    public Pile() : this(Mode.Stack)
    {
    }

    public Pile(uint extent) : this(Mode.Queue)
    {
        this.extent = (int) extent;
    }

    private Pile(Mode mode)
    {
        this.mode = mode;
        contents = new List<ICard>();
    }
    
    private enum Mode
    {
        Stack,
        Queue
    }

    public bool HasRoom => mode == Mode.Stack || contents.Count < Extent;
    public int Extent => mode == Mode.Stack ? contents.Count : extent;
    public int Count => contents.Count;

    public ICard Peek()
    {
        return contents.FirstOrDefault();
    }
    
    public ICard Pop()
    {
        if (contents.Count <= 0)
            return null;
        
        var poppedItem = contents[0];
        
        contents.RemoveAt(0);
        
        Index();

        return poppedItem;
    }

    public int? Insert(ICard card)
    {
        
        if (card == null || DoesContain(card))
            return default;
        
        int? newIndex = null;

        switch (mode)
        {
            case Mode.Stack:
                
                contents.Insert(0, card);

                newIndex = 0;
            
                break;
            case Mode.Queue:
                
                contents.Add(card);

                newIndex = contents.Count - 1;
            
                break;
        }

        return newIndex;
    }

    public bool Remove(ICard card)
    {
        return contents.Remove(card);
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