using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public interface IMenu
{
    IObservable<Unit> WhenClosed { get; }

    void Close();
}

[RequireComponent(typeof(Canvas), typeof(CanvasScaler))]
public abstract class Menu : MonoBehaviour, IMenu
{
    private readonly Subject<Unit> closing = new Subject<Unit>();

    public IObservable<Unit> WhenClosed => closing;

    public void Close()
    {
        closing.OnNext(Unit.Default);
        closing.OnCompleted();
        
        Destroy(gameObject);
    }
}