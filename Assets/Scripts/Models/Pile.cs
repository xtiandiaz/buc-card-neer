using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public interface IPile
{
    bool CanAdd { get; }
    Vector3 Position { set; }

    ICard Peek();
    void Add(ICard card);
    bool Remove(ICard card);
    ICard[] Take(int count);
    void Arrange();
    bool DoesContain(ICard card);
}

public class Pile : IPile
{
    public class Factory : PlaceholderFactory<ICardArrangement, uint?, Pile>
    {
    }
    
    private readonly List<ICard> contents = new List<ICard>();
    private readonly ICardArrangement arrangement;
    private readonly uint? extent;

    public Pile(ICardArrangement arrangement, uint? extent)
    {
        this.arrangement = arrangement;
        this.extent = extent;
    }

    public bool CanAdd => !extent.HasValue || contents.Count < extent.Value;

    public Vector3 Position { private get; set; }

    public ICard Peek()
    {
        return contents.FirstOrDefault();
    }

    public void Add(ICard card)
    {
        if (card == null || !CanAdd)
            return;
        
        contents.Add(card);
        
        Arrange();
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
        arrangement.Apply(contents, Position);
    }

    public bool DoesContain(ICard card)
    {
        return contents.Contains(card);
    }
}