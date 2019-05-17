using System;
using UniRx;

public interface IGame : IGameStatusNotifier
{
    void Reset();
    void End();
}

public class Game : IGame
{
    private readonly Subject<Unit> resetting = new Subject<Unit>();
    private readonly Subject<Unit> ending = new Subject<Unit>();

    public IObservable<Unit> WhenReset => resetting;
    public IObservable<Unit> WhenEnded => ending;
    
    public void Reset()
    {
        resetting.OnNext(Unit.Default);
    }

    public void End()
    {
        ending.OnNext(Unit.Default);
        ending.OnCompleted();
    }
}