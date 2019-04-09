using System;
using UnityEngine;
using Zenject;
using UniRx;

public class BoardView : MonoBehaviour
{
    [SerializeField] private Transform background;
    [SerializeField] private UserInteractionListener interactionListener;

    private new BoardCamera camera;
    private Rect viewRect;

    public IObservable<Direction> Move { get; private set; }

    [Inject]
    private void Construct(
        BoardCamera camera, 
        GameSettings settings
        )
    {
        this.camera = camera;

        Move = interactionListener.Swipe
            .ThrottleFirst(settings.MoveDuration.Multiply(0.9));
        
        var thisTransform = transform;
        var position = thisTransform.position;
        viewRect = camera.GetFrustumRect(position.z);

        background.localScale = new Vector3(viewRect.width, viewRect.height, 1f);
        background.position = Vector3.zero;
    }

    public void ParentAsNew(CardView cardView)
    {
        cardView.transform.SetParent(transform);
    }
}
