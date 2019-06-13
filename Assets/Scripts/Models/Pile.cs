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
    bool CanInsert { get; }
    IEnumerable<CardType> Types { get; }

    ICard Peek();
    bool Insert(ICard card, PileInsertionMode withMode);
    bool Remove(ICard card);
    void Sort(IComparer<ICard> usingComparer);
    void Arrange();
    bool DoesContain(ICard card);
}

public class Pile : IPile
{
    private readonly List<ICard> contents;
    private readonly IPileCardArrangement arrangement;
    private readonly int? extent;

    public Pile(IPileCardArrangement arrangement, int? extent)
    {
        this.arrangement = arrangement;
        this.extent = extent;
        contents = extent.HasValue ? new List<ICard>(extent.Value) : new List<ICard>();
    }

    public int Count => contents.Count;
    public bool CanInsert => !extent.HasValue || contents.Count < extent.Value;
    public IEnumerable<CardType> Types => contents.Select(card => card.Type).Distinct();

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

        arrangement.OnSorted(contents);
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