using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

public interface IDraggingManager
{
    IObservable<Unit> DragStarted { get; }
    IObservable<Vector3> Dragged { get; }
    IObservable<Vector3> DragEnded { get; }
    
    void ToggleDragging(bool on);
}

[RequireComponent(typeof(Collider2D))]
public class DraggingManager : MonoBehaviour, IDraggingManager
{
    private readonly Subject<Unit> dragStarted = new Subject<Unit>();
    private readonly Subject<Vector3> dragged = new Subject<Vector3>();
    private readonly Subject<Vector3> dragEnded = new Subject<Vector3>();
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private ObservableEventTrigger eventTrigger;
    private new ICamera camera;
    
    public IObservable<Unit> DragStarted => dragStarted;
    public IObservable<Vector3> Dragged => dragged;
    public IObservable<Vector3> DragEnded => dragEnded;

    [Inject]
    private void Construct(ICamera camera)
    {
        this.camera = camera;
        
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
                .Subscribe(_ => dragEnded.OnNext(lastWorldPos)));
    }

    private void OnDestroy()
    {
        disposables.Dispose();
    }
}