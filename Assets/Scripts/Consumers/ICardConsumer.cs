using System;

public interface ICardConsumer
{
    IObservable<ICard> WhenConsumed { get; }
    
    void SetProvider(ICardProvider provider);
    void Feed(ISlot slot);
}