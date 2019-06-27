using System;
using UniRx;
using UnityEngine;
using Zenject;

public interface ISlotView
{
    SlotType Type { get; }
    Transform Transform { get; }
    Bounds Bounds { get; }
    
    IObservable<Unit> WhenDraggingStarted { get; }
    IObservable<Vector3> WhenDragged { get; }
    IObservable<Vector3> WhenDraggingStopped { get; }
    IObservable<Direction> WhenSwiped { get; }

    void ToggleHighlight(bool on);
    void ToggleVisibility(bool on);
}

public class SlotView : MonoBehaviour, ISlotView
{
    [SerializeField] private SlotType type;
    [SerializeField] private SpriteRenderer faceRenderer;
    [SerializeField] private GestureListener gestureListener;

    private Color defaultFaceColor;
    private IBoardModel boardModel;

    public SlotType Type => type;
    public Transform Transform => transform;
    public Bounds Bounds => faceRenderer.bounds;
    
    public IObservable<Unit> WhenDraggingStarted => gestureListener.WhenDraggingStarted;
    public IObservable<Vector3> WhenDragged => gestureListener.WhenDragged;
    public IObservable<Vector3> WhenDraggingStopped => gestureListener.WhenDraggingEnded;
    public IObservable<Direction> WhenSwiped => gestureListener.WhenSwiped;

    [Inject]
    private void Initialize(
        IBoardModel boardModel, 
        IWorldPointProvider worldPointProvider
        )
    {
        this.boardModel = boardModel;

        defaultFaceColor = faceRenderer.color;
        
        gestureListener.Initialize(worldPointProvider);
    }

    public void ToggleHighlight(bool on)
    {
        if (on)
        {
            faceRenderer.color = Color.green;
            faceRenderer.sortingLayerName = boardModel.CardSortingLayerName;
            faceRenderer.sortingOrder = boardModel.FloatingCardSortingOrder - 1;
        }
        else
        {
            faceRenderer.color = defaultFaceColor;
            faceRenderer.sortingLayerName = boardModel.SlotSortingLayerName;
            faceRenderer.sortingOrder = 0;
        }
    }
    
    public void ToggleVisibility(bool on)
    {
        gameObject.SetActive(on);
    }
}