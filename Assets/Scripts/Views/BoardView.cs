using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;

public class BoardView : MonoBehaviour
{
    [SerializeField] private Transform background;
    [SerializeField] private UserInteractionListener interactionListener;
    [SerializeField] private List<CardSlotView> playSlots;
    [SerializeField] private List<CardSlotView> stashSlots;
    [SerializeField] private CardSlotView playerSlot;

    private new BoardCamera camera;
    private Rect viewRect;

    public IObservable<Direction> Move { get; private set; }
    
    public List<CardSlotView> PlaySlots => playSlots;
    public List<CardSlotView> StashSlots => stashSlots;
    public CardSlotView PlayerSlot => playerSlot;

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
