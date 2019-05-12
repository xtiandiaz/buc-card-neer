using System;

public interface ICardConsumer
{    
    void SetProvider(ICardProvider provider);
    void Consume(int count);
}