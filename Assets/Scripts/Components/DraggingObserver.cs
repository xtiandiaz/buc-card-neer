using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public interface IDraggingObserver
{
    IObservable<Unit> DraggingStart { get; }
    IObservable<Vector3> Dragging { get; }
    IObservable<Vector3> DraggingEnd { get; }
    IObservable<Unit> WhenDoubleClicked { get; }

    void Initialize(IWorldPointProvider worldPointProvider);
}

[RequireComponent(typeof(Collider2D))]
public class DraggingObserver : MonoBehaviour, IDraggingObserver
{
    private readonly Subject<Unit> draggingStart = new Subject<Unit>();
    private readonly Subject<Vector3> dragging = new Subject<Vector3>();
    private readonly Subject<Vector3> draggingEnd = new Subject<Vector3>();
    private readonly Subject<Unit> doubleClicking = new Subject<Unit>();
    private ObservableEventTrigger eventTrigger;
    
    public IObservable<Unit> DraggingStart => draggingStart;
    public IObservable<Vector3> Dragging => dragging;
    public IObservable<Vector3> DraggingEnd => draggingEnd;
    public IObservable<Unit> WhenDoubleClicked => doubleClicking;

    public void Initialize(IWorldPointProvider worldPointProvider)
    {
        eventTrigger = gameObject.AddComponent<ObservableEventTrigger>();

        var lastWorldPos = Vector3.zero;

        eventTrigger
            .OnBeginDragAsObservable()
            .Do(eventData => lastWorldPos = worldPointProvider.GetWorldPoint(eventData.position))
            .AsUnitObservable()
            .Subscribe(draggingStart)
            .AddTo(this);
        
        eventTrigger
            .OnDragAsObservable()
            .Select(eventData =>
            {
                var newWorldPos = worldPointProvider.GetWorldPoint(eventData.position);
                var deltaPos = newWorldPos - lastWorldPos;

                lastWorldPos = newWorldPos;
                
                return deltaPos;
            })
            .Subscribe(dragging)
            .AddTo(this);

        eventTrigger
            .OnEndDragAsObservable()
            .Select(eventData => worldPointProvider.GetWorldPoint(eventData.position))
            .Subscribe(draggingEnd)
            .AddTo(this);

        var clickStream = eventTrigger.OnPointerClickAsObservable();

        clickStream.Buffer(clickStream.Throttle(TimeSpan.FromMilliseconds(250)))
            .Where(xs => xs.Count >= 2)
            .Subscribe(_ => doubleClicking.OnNext(Unit.Default))
            .AddTo(this);
    }
}