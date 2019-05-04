using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public interface IDraggingObserver
{
    IObservable<Unit> DraggingStart { get; }
    IObservable<Vector3> Dragging { get; }
    IObservable<Vector3> DraggingEnd { get; }

    void Initialize(IWorldPointProvider worldPointProvider);
}

[RequireComponent(typeof(Collider2D))]
public class DraggingObserver : MonoBehaviour, IDraggingObserver
{
    private readonly Subject<Unit> draggingStart = new Subject<Unit>();
    private readonly Subject<Vector3> dragging = new Subject<Vector3>();
    private readonly Subject<Vector3> draggingEnd = new Subject<Vector3>();
    private ObservableEventTrigger eventTrigger;
    
    public IObservable<Unit> DraggingStart => draggingStart;
    public IObservable<Vector3> Dragging => dragging;
    public IObservable<Vector3> DraggingEnd => draggingEnd;

    public void Initialize(IWorldPointProvider worldPointProvider)
    {
        eventTrigger = gameObject.AddComponent<ObservableEventTrigger>();

        eventTrigger
            .OnBeginDragAsObservable()
            .Subscribe(eventData => draggingStart.OnNext(Unit.Default))
            .AddTo(this);
        
        eventTrigger
            .OnDragAsObservable()
            .Select(eventData => worldPointProvider.GetWorldPoint(eventData.position))
            .Subscribe(dragging.OnNext)
            .AddTo(this);

        eventTrigger
            .OnEndDragAsObservable()
            .Select(eventData => worldPointProvider.GetWorldPoint(eventData.position))
            .Subscribe(draggingEnd.OnNext)
            .AddTo(this);
    }
}