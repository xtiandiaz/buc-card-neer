using System;
using UniRx;

public interface ICardConsumer
{    
    void SetProvider(ICardProvider provider);
    void SetCapacity(int toValue);
    void Consume();
    IObservable<Unit> ConsumeAsObservable(int count, TimeSpan atIntervalsWithSpan);
    IObservable<Unit> FillToCapacity(TimeSpan atIntervalsWithSpan);
}