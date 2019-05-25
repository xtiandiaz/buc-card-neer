using System;
using UniRx;

public interface IMoveObserver
{
    void OnNext();
    void OnCompleted();
}

public interface IMoveObservable
{
    IObservable<Unit> WhenMoved { get; }
}

public class MoveRouter : IMoveObserver, IMoveObservable
{
    private readonly Subject<Unit> moving = new Subject<Unit>();
    
    public IObservable<Unit> WhenMoved => moving;

    public void OnNext()
    {
        moving.OnNext(Unit.Default);
    }

    public void OnCompleted()
    {
        moving.OnCompleted();
    }
}