using System.Collections.Generic;
using System.Linq;
using Zenject;

public enum PileInsertionMode
{
    Unshift,
    Push
}

public interface IPile
{
    int Count { get; }
    bool CanInsert { get; }

    ICard Peek();
    bool Insert(ICard card, PileInsertionMode withMode, bool andShouldThenArrange = true);
    bool Remove(ICard card, bool andShouldThenArrange = true);
    ICard[] Take(int count);
    void Arrange();
    bool DoesContain(ICard card);
}

public class Pile : IPile
{
    private readonly List<ICard> contents;
    private readonly ICardArrangement arrangement;
    private readonly int? extent;

    public Pile(ICardArrangement arrangement, int? extent)
    {
        this.arrangement = arrangement;
        this.extent = extent;
        contents = extent.HasValue ? new List<ICard>(extent.Value) : new List<ICard>();
    }

    public int Count => contents.Count;
    public bool CanInsert => !extent.HasValue || contents.Count < extent.Value;

    public ICard Peek()
    {
        return contents.FirstOrDefault();
    }

    public bool Insert(ICard card, PileInsertionMode withMode, bool andShouldThenArrange = true)
    {
        if (card == null || !CanInsert || DoesContain(card))
            return false;

        switch (withMode)
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
        
        if (andShouldThenArrange)
            Arrange();

        return true;
    }

    public bool Remove(ICard card, bool andShouldThenArrange = true)
    {
        var didRemove = contents.Remove(card);
        
        if (didRemove && andShouldThenArrange)
            Arrange();

        return didRemove;
    }

    public ICard[] Take(int count)
    {
        var taken = contents.Take(count).ToArray();

        Arrange();
        
        return taken;
    }

    public void Arrange()
    {
        arrangement?.Apply(contents, extent);
    }

    public bool DoesContain(ICard card)
    {
        return contents.Contains(card);
    }
}