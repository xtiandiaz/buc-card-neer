using System;

public interface ICardProvider
{
    IObservable<ICard> WhenProvided { get; }
    bool IsExhausted { get; }
    
    ICard Provide();
}