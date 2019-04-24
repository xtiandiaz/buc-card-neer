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
    [SerializeField] private List<CardSlotView> slotViews;
    [SerializeField] private OceanView ocean;
    [SerializeField] private OwnShipView ownShip;
    [SerializeField] private MerchantShipView merchantShip;
    [SerializeField] private PirateShipView pirateShip;

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

    public void Initialize()
    {
        var thisTransform = transform;
        var position = thisTransform.position;

        viewRect = camera.GetFrustumRect(position.z);
        
        ownShip.Initialize();
        ocean.Initialize(viewRect.height);
        
        thisTransform.position = Vector3.down * (viewRect.height * 0.5f);
        
        ownShip.transform.localPosition = Vector3.up * ownShip.Height * 0.5f;
        ocean.transform.localPosition = Vector3.up * (ownShip.Height + ocean.Height);

        //thisTransform.position += Vector3.down * settings.CardSize.y * 0.5f;
    }

    public void Set(BoardMode mode)
    {
        merchantShip.gameObject.SetActive(false);
        pirateShip.gameObject.SetActive(false);
        
        switch (mode)
        {
            case BoardMode.Seafaring:

                ocean.ToggleProjection(true);
                
                break;
            case BoardMode.Trade:
                
                ocean.ToggleProjection(false);
                merchantShip.gameObject.SetActive(true);
                merchantShip.Dock(Vector3.up * (ownShip.Height + merchantShip.Height * 0.5f));
                
                break;
            case BoardMode.Combat:
                
                ocean.ToggleProjection(false);
                pirateShip.gameObject.SetActive(true);
                pirateShip.Dock(Vector3.up * (ownShip.Height + pirateShip.Height * 0.5f + 1f));
                
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }
    }
}
