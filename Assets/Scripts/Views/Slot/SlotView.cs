using System;
using UniRx;
using UnityEngine;
using Zenject;

public interface ISlotView
{
    ISlotSettings Settings { get; }
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
    [SerializeField] private SlotSettings settings;
    [SerializeField] private SpriteRenderer faceRenderer;
    [SerializeField] private GestureListener gestureListener;

    private Color defaultFaceColor;
    private BoardLayoutSettings layoutSettings;

    public ISlotSettings Settings => settings;
    public Transform Transform => transform;
    public Bounds Bounds => faceRenderer.bounds;
    
    public IObservable<Unit> WhenDraggingStarted => gestureListener.WhenDraggingStarted;
    public IObservable<Vector3> WhenDragged => gestureListener.WhenDragged;
    public IObservable<Vector3> WhenDraggingStopped => gestureListener.WhenDraggingEnded;
    public IObservable<Direction> WhenSwiped => gestureListener.WhenSwiped;

    [Inject]
    private void Initialize(
        BoardLayoutSettings layoutSettings, 
        IWorldPointProvider worldPointProvider
        )
    {
        this.layoutSettings = layoutSettings;

        defaultFaceColor = faceRenderer.color;
        
        gestureListener.Initialize(worldPointProvider);
    }

    public void ToggleHighlight(bool on)
    {
        if (on)
        {
            faceRenderer.color = settings.HighlightColor;
            faceRenderer.sortingLayerName = layoutSettings.CardSortingLayerName;
            faceRenderer.sortingOrder = layoutSettings.FloatingCardSortingOrder - 1;
        }
        else
        {
            faceRenderer.color = defaultFaceColor;
            faceRenderer.sortingLayerName = layoutSettings.SlotSortingLayerName;
            faceRenderer.sortingOrder = 0;
        }
    }
    
    public void ToggleVisibility(bool on)
    {
        gameObject.SetActive(on);
    }
}