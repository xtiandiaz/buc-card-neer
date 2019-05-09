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
    bool CanInsert { get; }

    ICard Peek();
    bool Insert(ICard card, PileInsertionMode withMode);
    bool Remove(ICard card);
    ICard[] Take(int count);
    void Arrange();
    bool DoesContain(ICard card);
}

public class Pile : IPile
{
    public class Factory : PlaceholderFactory<ICardArrangement, int?, Pile>
    {
    }
    
    private readonly List<ICard> contents;
    private readonly ICardArrangement arrangement;
    private readonly int? extent;

    public Pile(ICardArrangement arrangement, int? extent)
    {
        this.arrangement = arrangement;
        this.extent = extent;
        contents = extent.HasValue ? new List<ICard>(extent.Value) : new List<ICard>();
    }

    public bool CanInsert => !extent.HasValue || contents.Count < extent.Value;

    public ICard Peek()
    {
        return contents.FirstOrDefault();
    }

    public bool Insert(ICard card, PileInsertionMode withMode)
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
        
        Arrange();

        return true;
    }

    public bool Remove(ICard card)
    {
        var didRemove = contents.Remove(card);
        
        if (didRemove)
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
        arrangement.Apply(contents, extent);
    }

    public bool DoesContain(ICard card)
    {
        return contents.Contains(card);
    }
}