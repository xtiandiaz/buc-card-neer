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

    public IObservable<Vector2> PointerDown
    {
    	get 
    	{
    		return pointerDown;
    	}
    }

    public IObservable<Vector2> Tap
    {
    	get 
    	{
    		return tap;
    	}
    }

    public IObservable<Direction> Swipe
    {
    	get 
    	{
    		return swipe;
    	}
    }

	private void Awake() 
	{
		var eventTrigger = this.gameObject.AddComponent<ObservableEventTrigger>(); 

		var onPointerDownObservable = eventTrigger
			.OnPointerDownAsObservable()
			.TakeUntilDisable(this)
			.Select(eventData => eventData.pressPosition)
			.Share();

		onPointerDownObservable
			.Subscribe(pointerDown.OnNext);

		var onTapObservable = eventTrigger
			.OnPointerClickAsObservable()
			.TakeUntilDisable(this)
			.Select(eventData => eventData.pressPosition)
			.Share();

		onTapObservable
			.Subscribe(tap.OnNext);

        eventTrigger
			.OnBeginDragAsObservable() 
			.TakeUntilDisable(this) 
			.Where(eventData => eventData.pointerDrag.gameObject == this.gameObject) 
			.Select(eventData => eventData.position) 
			.Subscribe(position => 
				{ 
					this.beginPosition = position; 
					this.beginTime = DateTime.Now;
				});

		var onEndDragObservable = eventTrigger
			.OnEndDragAsObservable() 
            .TakeUntilDisable(this) 
            .Where(eventData => (DateTime.Now - this.beginTime).TotalSeconds < this.swipeThresholdInSeconds) 
            .Select(eventData => eventData.position) 
            .Share();

        onEndDragObservable
        	.Select(position => 
        		{
        			var deltaX = Mathf.Abs(position.x - beginPosition.x);
                    var deltaY = Mathf.Abs(position.y - beginPosition.y);

                    if (deltaX > deltaY && deltaX > this.swipeThresholdDistance)
                    {
                        return position.x > beginPosition.x ? Direction.Right : Direction.Left;
                    }
                    else if (deltaY >= deltaX && deltaY > this.swipeThresholdDistance)
                    {
                        return position.y > beginPosition.y ? Direction.Up : Direction.Down;
                    }

        			return Direction.None;
        		})
        	.Where(dir => dir != Direction.None)
        	.Subscribe(swipe.OnNext);
	}
}