using System;
using UniRx;
using UnityEngine;
using Zenject;

public interface ISlotView
{
    SlotType Type { get; }
    Transform Transform { get; }
    Bounds Bounds { get; }
    
    IObservable<Vector2> WhenPressed { get; }
    IObservable<Unit> WhenReleased { get; }
    IObservable<Unit> WhenDraggingStarted { get; }
    IObservable<Vector2> WhenDragged { get; }
    IObservable<Vector3> WhenDraggingStopped { get; }

    void ToggleHighlight(bool on);
    void ToggleVisibility(bool on);
}

public class SlotView : MonoBehaviour, ISlotView
{
    [SerializeField] private SlotType type = default;
    [SerializeField] private SpriteRenderer faceRenderer = default;
    [SerializeField] private SpriteRenderer iconRenderer = default;
    [SerializeField] private GestureListener gestureListener = default;

    private Color defaultFaceColor;
    private IBoardModel boardModel;

    public SlotType Type => type;
    public Transform Transform => transform;
    public Bounds Bounds => faceRenderer.bounds;

    public IObservable<Vector2> WhenPressed => gestureListener.WhenPressed;
    public IObservable<Unit> WhenReleased => gestureListener.WhenReleased;
    public IObservable<Unit> WhenDraggingStarted => gestureListener.WhenDraggingStarted;
    public IObservable<Vector2> WhenDragged => gestureListener.WhenDragged;
    public IObservable<Vector3> WhenDraggingStopped => gestureListener.WhenDraggingEnded;

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

            if (iconRenderer != null)
                iconRenderer.color = Color.green;
            
            faceRenderer.sortingLayerName = boardModel.CardSortingLayerName;
            faceRenderer.sortingOrder = boardModel.FloatingCardSortingOrder - 1;
        }
        else
        {
            if (iconRenderer != null)
                iconRenderer.color = defaultFaceColor;
            
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