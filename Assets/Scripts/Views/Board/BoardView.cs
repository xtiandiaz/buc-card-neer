using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Zenject;
using UniRx;

public interface IBoardView
{
    IShipView[] Ships { get; }
    IDeck[] Decks { get; }
    IOceanView Ocean { get; }

    void Initialize();
    void Set(BoardMode mode);
}

public class BoardView : MonoBehaviour, IBoardView
{
    private const float ProjectionDurationInSeconds = 1f;
    private const float DockingDurationInSeconds = 1f;
    private const float SailingDurationInSeconds = 0.75f;
    
    [SerializeField] private Deck[] decks;
    [SerializeField] private OceanView ocean;
    [SerializeField] private ShipPlayerView shipPlayer;
    [SerializeField] private ShipMerchantView shipMerchant;
    [SerializeField] private ShipPirateView shipPirate;

    private BoardCamera boardCamera;
    private GameSettings settings;
    private Rect viewRect;
    private BoardMode lastMode;

    public IShipView[] Ships => new IShipView[] {shipPlayer, shipMerchant, shipPirate};
    public IDeck[] Decks => decks;
    public IOceanView Ocean => ocean;

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
        shipPlayer.Initialize(viewRect.height);
        shipPirate.Initialize(viewRect.height);
        shipMerchant.Initialize(viewRect.height);
        
        thisTransform.position = Vector3.down * (viewRect.height * 0.5f);
        
        shipPlayer.transform.localPosition = Vector3.up * shipPlayer.Height * 0.5f;
        ocean.transform.localPosition = Vector3.up * (shipPlayer.Height + ocean.Height);
        
        shipMerchant.gameObject.SetActive(false);
        shipPirate.gameObject.SetActive(false);

        //thisTransform.position += Vector3.down * settings.CardSize.y * 0.5f;
    }

    public void Set(BoardMode mode)
    {
        var projectionDelay = 0f;
        
        switch (lastMode)
        {
            case BoardMode.Trade:
                
                shipMerchant.SetSail(SailingDurationInSeconds);
                projectionDelay = SailingDurationInSeconds * 0.85f;
                
                break;
            case BoardMode.Combat:
                
                shipPirate.SetSail(SailingDurationInSeconds);
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
                shipMerchant.gameObject.SetActive(true);
                shipMerchant.Dock(
                    Vector3.up * (shipPlayer.Height + shipMerchant.Height * 0.5f), 
                    DockingDurationInSeconds,
                    ProjectionDurationInSeconds * 0.85f);
                
                break;
            case BoardMode.Combat:
                
                ocean.ToggleProjection(false, DockingDurationInSeconds);
                shipPirate.gameObject.SetActive(true);
                shipPirate.Dock(
                    Vector3.up * (shipPlayer.Height + shipPirate.Height * 0.5f + 1f),
                    DockingDurationInSeconds,
                    ProjectionDurationInSeconds * 0.85f);
                
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }

        lastMode = mode;
    }
}
