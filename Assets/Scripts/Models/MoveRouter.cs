using System;
using UniRx;

public interface IMoveDispatcher
{
    IObservable<Unit> WhenPlayerMoved { get; }
}

public interface IMoveRouter : IMoveDispatcher
{
    void OnNext();
}

public class MoveRouter : IMoveRouter
{
    private readonly Subject<Unit> moving = new Subject<Unit>();
    
    public IObservable<Unit> WhenPlayerMoved => moving;

    public void OnNext()
    {
        moving.OnNext(Unit.Default);
    }
}