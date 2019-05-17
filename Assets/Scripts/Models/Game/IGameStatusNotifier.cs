using System;
using UniRx;

public interface IGameStatusNotifier
{
    IObservable<Unit> WhenReset { get; }
    IObservable<Unit> WhenEnded { get; }
}