using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public interface IDraggingManager
{
    IObservable<Unit> DragStarted { get; }
    IObservable<Vector3> Dragged { get; }
    IObservable<Vector3> DragEnded { get; }

    void Initialize(IWorldPointProvider worldPointProvider);
}

[RequireComponent(typeof(Collider2D))]
public class DraggingManager : MonoBehaviour, IDraggingManager
{
    private readonly Subject<Unit> dragStarted = new Subject<Unit>();
    private readonly Subject<Vector3> dragged = new Subject<Vector3>();
    private readonly Subject<Vector3> dragEnded = new Subject<Vector3>();
    private ObservableEventTrigger eventTrigger;
    
    public IObservable<Unit> DragStarted => dragStarted;
    public IObservable<Vector3> Dragged => dragged;
    public IObservable<Vector3> DragEnded => dragEnded;

    public void Initialize(IWorldPointProvider worldPointProvider)
    {
        eventTrigger = gameObject.AddComponent<ObservableEventTrigger>();
        
        var lastWorldPos = Vector3.zero;

        eventTrigger
            .OnBeginDragAsObservable()
            .Subscribe(eventData =>
            {
                lastWorldPos = worldPointProvider.GetWorldPoint(eventData.position);

                dragStarted.OnNext(Unit.Default);
            })
            .AddTo(this);
        
        eventTrigger
            .OnDragAsObservable()
            .Select(eventData =>
            {
                var worldPos = worldPointProvider.GetWorldPoint(eventData.position);
                var delta = worldPos - lastWorldPos;
                lastWorldPos = worldPos;

                return delta;
            })
            .Subscribe(dragged.OnNext)
            .AddTo(this);

        eventTrigger
            .OnEndDragAsObservable()
            .Select(_ => lastWorldPos)
            .Subscribe(dragEnded.OnNext)
            .AddTo(this);
    }
}