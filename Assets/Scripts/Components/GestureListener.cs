using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GestureListener : MonoBehaviour
{
    private readonly Subject<Vector2> pressing = new Subject<Vector2>();
    private readonly Subject<Unit> releasing = new Subject<Unit>();
    private readonly Subject<Unit> draggingStart = new Subject<Unit>();
    private readonly Subject<Vector2> dragging = new Subject<Vector2>();
    private readonly Subject<Vector3> draggingEnd = new Subject<Vector3>();
    private readonly Subject<Direction> swiping = new Subject<Direction>();

    [SerializeField] private float swipeThresholdInSeconds = 1.0f;
    [SerializeField] public float swipeThresholdDistance = 100.0f;
    private ObservableEventTrigger eventTrigger;
    private Vector2 startPosition;
    private DateTime startTime;

    public IObservable<Vector2> WhenPressed => pressing;
    public IObservable<Unit> WhenReleased => releasing;
    public IObservable<Unit> WhenDraggingStarted => draggingStart;
    public IObservable<Vector2> WhenDragged => dragging;
    public IObservable<Vector3> WhenDraggingEnded => draggingEnd;
    public IObservable<Direction> WhenSwiped => swiping;

    public void Initialize(IWorldPointProvider worldPointProvider)
    {
        eventTrigger = gameObject.AddComponent<ObservableEventTrigger>();

        var lastWorldPos = Vector3.zero;

        eventTrigger
            .OnPointerDownAsObservable()
            .TakeUntilDisable(this)
            .Subscribe(eventData => pressing.OnNext(eventData.position))
            .AddTo(this);
        
        eventTrigger
            .OnPointerUpAsObservable()
            .TakeUntilDisable(this)
            .AsUnitObservable()
            .Subscribe(releasing.OnNext)
            .AddTo(this);

        eventTrigger
            .OnBeginDragAsObservable()
            .TakeUntilDisable(this)
            .Do(eventData =>
            {
                lastWorldPos = worldPointProvider.GetWorldPoint(eventData.position);
                startPosition = eventData.position;
                startTime = DateTime.Now;
            })
            .AsUnitObservable()
            .Subscribe(draggingStart)
            .AddTo(this);

        eventTrigger
            .OnDragAsObservable()
            .TakeUntilDisable(this)
            .Select(eventData => eventData.position)
            .Subscribe(dragging)
            .AddTo(this);

        var dragEndObservable = eventTrigger
            .OnEndDragAsObservable()
            .TakeUntilDisable(this)
            .Select(eventData => eventData.position)
            .Share();

        dragEndObservable
            .Select(screenPos => worldPointProvider.GetWorldPoint(screenPos, 0))
            .Subscribe(draggingEnd)
            .AddTo(this);

        dragEndObservable
            .Where(eventData => (DateTime.Now - startTime).TotalSeconds < swipeThresholdInSeconds)
            .Select(screenPosition =>
            {
                var deltaX = Mathf.Abs(screenPosition.x - startPosition.x);
                var deltaY = Mathf.Abs(screenPosition.y - startPosition.y);

                if (deltaX > deltaY && deltaX > swipeThresholdDistance)
                {
                    return screenPosition.x > startPosition.x ? Direction.Right : Direction.Left;
                }

                if (deltaY >= deltaX && deltaY > swipeThresholdDistance)
                {
                    return screenPosition.y > startPosition.y ? Direction.Up : Direction.Down;
                }

                return Direction.None;
            })
            .Subscribe(swiping)
            .AddTo(this);
    }

    private void OnDestroy()
    {
        pressing.Dispose();
        releasing.Dispose();
        dragging.Dispose();
        draggingStart.Dispose();
        draggingEnd.Dispose();
        swiping.Dispose();
    }
}