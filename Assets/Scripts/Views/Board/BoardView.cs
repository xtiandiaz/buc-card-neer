using UnityEngine;
using Zenject;

public interface IBoardView
{
    IShipView[] Ships { get; }
    IDeck[] Decks { get; }
    ISeaView Sea { get; }

    void Initialize();
}

public class BoardView : MonoBehaviour, IBoardView
{
    [SerializeField] private Deck[] decks;
    [SerializeField] private SeaView sea;
    [SerializeField] private ShipPlayerView shipPlayer;
    [SerializeField] private ShipMerchantView shipMerchant;
    [SerializeField] private ShipPirateView shipPirate;

    private BoardCamera boardCamera;
    private Rect viewRect;
    private BoardMode lastMode;

    public IShipView[] Ships => new IShipView[] {shipPlayer, shipMerchant, shipPirate};
    public IDeck[] Decks => decks;
    public ISeaView Sea => sea;
    
    public Vector3 PirateDockingPosition { get; private set; }
    public Vector3 MerchantDockingPosition { get; private set; }
    public Vector3 PirateSailingDestination { get; private set; }
    public Vector3 MerchantSailingDestination { get; private set; }

    [Inject]
    private void Construct(
        BoardCamera camera
        )
    {
        boardCamera = camera;
    }

    public void Initialize()
    {
        var thisTransform = transform;
        var position = thisTransform.position;

        viewRect = boardCamera.GetFrustumRect(position.z);

        sea.Initialize(viewRect.height);

        thisTransform.position = Vector3.down * (viewRect.height * 0.5f);

        shipPlayer.transform.localPosition = Vector3.up * shipPlayer.Height * 0.5f;
        sea.transform.localPosition = Vector3.up * (shipPlayer.Height + sea.Height);

        PirateDockingPosition = Vector3.up * (shipPlayer.Height + shipPirate.Height * 0.5f + 1f);
        MerchantDockingPosition = Vector3.up * (shipPlayer.Height + shipMerchant.Height * 0.5f);
        PirateSailingDestination = Vector3.up * (viewRect.height + shipPirate.Height * 0.5f);
        MerchantSailingDestination = Vector3.up * (viewRect.height + shipMerchant.Height * 0.5f);
    }
}
