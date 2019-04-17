using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class UserInteractionListener : MonoBehaviour
{
	[SerializeField]
	private float swipeThresholdInSeconds = 1.0f;
	[SerializeField]
	public float swipeThresholdDistance = 100.0f;

	private Vector2 beginPosition;
    private DateTime beginTime;

    private Subject<Vector2> pointerDown = new Subject<Vector2>();
    private Subject<Vector2> tap = new Subject<Vector2>();
    private Subject<Direction> swipe = new Subject<Direction>();

    public IObservable<Vector2> PointerDown => pointerDown;
    public IObservable<Vector2> Tap => tap;
    public IObservable<Direction> Swipe => swipe;

    private void Awake() 
	{
		var eventTrigger = gameObject.AddComponent<ObservableEventTrigger>(); 

		var onPointerDownObservable = eventTrigger
			.OnPointerDownAsObservable()
			.TakeUntilDisable(this)
			.Select(eventData => eventData.pressPosition)
			.Share();

		onPointerDownObservable
			.Subscribe(pointerDown.OnNext)
			.AddTo(this);

		var onTapObservable = eventTrigger
			.OnPointerClickAsObservable()
			.TakeUntilDisable(this)
			.Select(eventData => eventData.pressPosition)
			.Share();

		onTapObservable
			.Subscribe(tap.OnNext);

		var onBeginDragObservable = eventTrigger
			.OnBeginDragAsObservable()
			.TakeUntilDisable(this)
			.Where(eventData => eventData.pointerDrag.gameObject == gameObject)
			.Select(eventData => eventData.position)
			.Share();

		onBeginDragObservable
			.Subscribe(position =>
			{
				beginPosition = position;
				beginTime = DateTime.Now;
			})
			.AddTo(this);

		var onEndDragObservable = eventTrigger
			.OnEndDragAsObservable() 
            .TakeUntilDisable(this) 
            .Select(eventData => eventData.position) 
            .Share();

        onEndDragObservable
	        .Where(eventData => (DateTime.Now - beginTime).TotalSeconds < swipeThresholdInSeconds)
        	.Select(position => 
        		{
        			var deltaX = Mathf.Abs(position.x - beginPosition.x);
                    var deltaY = Mathf.Abs(position.y - beginPosition.y);

                    if (deltaX > deltaY && deltaX > this.swipeThresholdDistance)
                    {
                        return position.x > beginPosition.x ? Direction.Right : Direction.Left;
                    }
                    
                    if (deltaY >= deltaX && deltaY > this.swipeThresholdDistance)
                    {
                        return position.y > beginPosition.y ? Direction.Up : Direction.Down;
                    }

        			return Direction.None;
        		})
        	.Where(dir => dir != Direction.None)
        	.Subscribe(swipe.OnNext)
            .AddTo(this);
	}
}