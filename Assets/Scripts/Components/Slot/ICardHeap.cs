using System;
using System.Collections.Generic;

public interface ICardHeap
{
    int Count { get; }
    bool HasRoom { get; }
    
    int? Insert(ICard card);
    ICard Peek();
    ICard Pop();
    IEnumerable<T> Map<T>(Func<ICard, int, T> byFunction);
    bool DoesContain(ICard card);
    
}