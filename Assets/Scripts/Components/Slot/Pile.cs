using System;
using System.Collections.Generic;
using System.Linq;

public enum PileInsertionMode
{
    Unshift,
    Push
}

public interface IPile
{
    int Count { get; }
    IEnumerable<CardType> Types { get; }

    ICard Peek();
    bool Insert(ICard card);
    bool Remove(ICard card);
    void Sort(IComparer<ICard> usingComparer);
    IEnumerable<T> Map<T>(Func<ICard, int, T> byFunction);
    bool DoesContain(ICard card);
}

public class Pile : IPile
{
    private readonly PileInsertionMode insertionMode;
    private readonly List<ICard> contents;

    public Pile(PileInsertionMode insertionMode)
    {
        this.insertionMode = insertionMode;
        contents = new List<ICard>();
    }

    public int Count => contents.Count;
    public IEnumerable<CardType> Types => contents.Select(card => card.Type).Distinct();

    public ICard Peek()
    {
        return contents.LastOrDefault();
    }

    public bool Insert(ICard card)
    {
        if (card == null || DoesContain(card))
            return false;

        switch (insertionMode)
        {
            case PileInsertionMode.Unshift:
                contents.Insert(0, card);
                break;
            case PileInsertionMode.Push:
                contents.Add(card);
                break;
            default:
                return false;
        }

        return true;
    }

    public bool Remove(ICard card)
    {
        return contents.Remove(card);
    }

    public void Sort(IComparer<ICard> usingComparer)
    {
        if (contents.Count < 2)
            return;

        contents.Sort(usingComparer);
    }

    public IEnumerable<T> Map<T>(Func<ICard, int, T> byFunction)
    {
        return contents.Select(byFunction);
    }

    public bool DoesContain(ICard card)
    {
        return contents.Contains(card);
    }
}