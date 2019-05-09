using System;

public interface ICardProvider
{
    IObservable<ICard> WhenProvided { get; }
    
    ICard Provide();
}