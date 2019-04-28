using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DraggingManager : MonoBehaviour
{
    private readonly Subject<Unit> dragStarted = new Subject<Unit>();
    private readonly Subject<Vector3> dragged = new Subject<Vector3>();
    private readonly Subject<Unit> dragEnded = new Subject<Unit>();
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private ObservableEventTrigger eventTrigger;
    private new ICamera camera;
    
    public IObservable<Unit> DragStarted => dragStarted;
    public IObservable<Vector3> Dragged => dragged;
    public IObservable<Unit> DragEnded => dragEnded;

    public void Initialize(ICamera withCamera)
    {
        camera = withCamera;
        eventTrigger = gameObject.AddComponent<ObservableEventTrigger>();
    }

    public void ToggleDragging(bool on)
    {
        disposables.Clear();
        
        if (!on)
            return;
        
        var lastWorldPos = Vector3.zero;

        disposables.Add(
            eventTrigger
                .OnBeginDragAsObservable()
                .Subscribe(eventData =>
                {
                    lastWorldPos = camera.GetWorldPoint(eventData.position);
                    
                    dragStarted.OnNext(Unit.Default);
                }));

        disposables.Add(
            eventTrigger
                .OnDragAsObservable()
                .Select(eventData =>
                {
                    var worldPos = camera.GetWorldPoint(eventData.position);
                    var delta = worldPos - lastWorldPos;
                    lastWorldPos = worldPos;

                    return delta;
                })
                .Subscribe(dragged.OnNext));

        disposables.Add(
            eventTrigger
                .OnEndDragAsObservable()
                .Subscribe(_ => dragEnded.OnNext(Unit.Default)));
    }

    private void OnDestroy()
    {
        disposables.Dispose();
    }
}