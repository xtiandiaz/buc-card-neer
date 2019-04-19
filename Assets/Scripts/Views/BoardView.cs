using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;

public interface IBoardView
{
    IEnumerable<CardSlotView> SlotViews { get; }

    void Parent(Transform child);
}

public class BoardView : MonoBehaviour, IBoardView
{
    [SerializeField] private Transform background;
    [SerializeField] private List<CardSlotView> slotViews;

    private new BoardCamera camera;
    private GameSettings settings;
    private Rect viewRect;

    public IEnumerable<CardSlotView> SlotViews => slotViews;

    [Inject]
    private void Construct(
        BoardCamera camera, 
        GameSettings settings
        )
    {
        this.camera = camera;
        this.settings = settings;
    }

    private void Awake()
    {
        var thisTransform = transform;
        var position = thisTransform.position;
        var height = settings.CardSize.y * 2f + 1f; // TODO Turn dynamic
        
        viewRect = camera.GetFrustumRect(position.z);

        background.localScale = new Vector3(viewRect.width, viewRect.height, 1f);
        background.position = Vector3.zero;

        thisTransform.position = Vector3.down * ((viewRect.height - height) * 0.5f - settings.BoardMargins.y);
    }

    public void Parent(Transform child)
    {
        child.SetParent(transform, false);
    }
}
