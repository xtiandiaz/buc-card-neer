using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Zenject;
using UniRx;

public interface IBoardView
{
    IEnumerable<CardSlotView> SlotViews { get; }

    void Initialize();
    void Set(BoardMode mode);
}

public class BoardView : MonoBehaviour, IBoardView
{
    private const float ProjectionDurationInSeconds = 1f;
    private const float DockingDurationInSeconds = 1f;
    private const float SailingDurationInSeconds = 0.75f;

    [SerializeField] private List<CardSlotView> slotViews;
    [SerializeField] private OceanView ocean;
    [SerializeField] private OwnShipView ownShip;
    [SerializeField] private MerchantShipView merchantShip;
    [SerializeField] private PirateShipView pirateShip;

    private BoardCamera boardCamera;
    private GameSettings settings;
    private Rect viewRect;
    private BoardMode lastMode;

    public IEnumerable<CardSlotView> SlotViews => slotViews;

    [Inject]
    private void Construct(
        BoardCamera camera, 
        GameSettings settings
        )
    {
        boardCamera = camera;
        this.settings = settings;
    }

    public void Initialize()
    {
        var thisTransform = transform;
        var position = thisTransform.position;

        viewRect = boardCamera.GetFrustumRect(position.z);
        
        ocean.Initialize(viewRect.height);
        ownShip.Initialize(viewRect.height);
        pirateShip.Initialize(viewRect.height);
        merchantShip.Initialize(viewRect.height);
        
        thisTransform.position = Vector3.down * (viewRect.height * 0.5f);
        
        ownShip.transform.localPosition = Vector3.up * ownShip.Height * 0.5f;
        ocean.transform.localPosition = Vector3.up * (ownShip.Height + ocean.Height);
        
        merchantShip.gameObject.SetActive(false);
        pirateShip.gameObject.SetActive(false);

        //thisTransform.position += Vector3.down * settings.CardSize.y * 0.5f;
    }

    public void Set(BoardMode mode)
    {
        var projectionDelay = 0f;
        
        switch (lastMode)
        {
            case BoardMode.Trade:
                
                merchantShip.SetSail(SailingDurationInSeconds);
                projectionDelay = SailingDurationInSeconds * 0.85f;
                
                break;
            case BoardMode.Combat:
                
                pirateShip.SetSail(SailingDurationInSeconds);
                projectionDelay = SailingDurationInSeconds * 0.85f;
                
                break;
        }
        
        switch (mode)
        {
            case BoardMode.Seafaring:

                ocean.ToggleProjection(true, ProjectionDurationInSeconds, projectionDelay);
                
                break;
            case BoardMode.Trade:
                
                ocean.ToggleProjection(false, DockingDurationInSeconds);
                merchantShip.gameObject.SetActive(true);
                merchantShip.Dock(
                    Vector3.up * (ownShip.Height + merchantShip.Height * 0.5f), 
                    DockingDurationInSeconds,
                    ProjectionDurationInSeconds * 0.85f);
                
                break;
            case BoardMode.Combat:
                
                ocean.ToggleProjection(false, DockingDurationInSeconds);
                pirateShip.gameObject.SetActive(true);
                pirateShip.Dock(
                    Vector3.up * (ownShip.Height + pirateShip.Height * 0.5f + 1f),
                    DockingDurationInSeconds,
                    ProjectionDurationInSeconds * 0.85f);
                
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }

        lastMode = mode;
    }
}
