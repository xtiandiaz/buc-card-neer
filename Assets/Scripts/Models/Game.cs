using System;
using UniRx;

public interface IGame
{
    IObservable<Unit> WhenReset { get; }
    
    void Reset();
}

public class Game : IGame
{
    private readonly Subject<Unit> resetting = new Subject<Unit>();

    public IObservable<Unit> WhenReset => resetting;
    
    public void Reset()
    {
        resetting.OnNext(Unit.Default);
    }
}