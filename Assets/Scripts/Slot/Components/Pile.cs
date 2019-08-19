using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

public interface IPile
{
    int Extent { get; }
    int Count { get; }
    bool HasRoom { get; }
    
    IObservable<int> WhenCountChanged { get; }
    
    int? Insert(ICard card);
    int? InsertReverse(ICard card);
    bool Remove(ICard card);
    ICard Peek();
    ICard Pop();
    IEnumerable<T> Map<T>(Func<ICard, int, T> byFunction);
    void ForEach(Action<ICard, int> applyAction);
    bool DoesContain(ICard card);
}

public class Pile : IPile
{
    private readonly Mode mode;
    private readonly ReactiveCollection<ICard> contents;
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
        contents = new ReactiveCollection<ICard>();
    }
    
    private enum Mode
    {
        Stack,
        Queue
    }

    public bool HasRoom => mode == Mode.Stack || contents.Count < Extent;
    public int Extent => mode == Mode.Stack ? contents.Count : extent;
    public int Count => contents.Count;

    public IObservable<int> WhenCountChanged => contents.ObserveCountChanged();

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

        return poppedItem;
    }

    public int? Insert(ICard card)
    {
        if (card == null || DoesContain(card))
            return default;

        switch (mode)
        {
            case Mode.Stack:
                
                contents.Insert(0, card);
                
                return 0;
            case Mode.Queue:
                
                contents.Add(card);
                
                return contents.Count - 1;
        }

        return default;
    }

    public int? InsertReverse(ICard card)
    {
        if (card == null || DoesContain(card))
            return default;

        switch (mode)
        {
            case Mode.Queue:
                
                contents.Insert(0, card);

                return 0;
                
            case Mode.Stack:
                
                contents.Add(card);

                return contents.Count - 1;
        }

        return default;
    }
    
    public bool Remove(ICard card)
    {
        return contents.Remove(card);
    }

    public IEnumerable<T> Map<T>(Func<ICard, int, T> byFunction)
    {
        return contents.Select(byFunction);
    }

    public void ForEach(Action<ICard, int> applyAction)
    {
        for (var i = 0; i < contents.Count; i++)
            applyAction(contents[i], i);
    }

    public bool DoesContain(ICard card)
    {
        return contents.Contains(card);
    }
}