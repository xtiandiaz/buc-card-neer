using UnityEngine;
using Zenject;

public interface IBoardView
{
    IShipView[] Ships { get; }
    IDeck[] Decks { get; }
    ISeaView Sea { get; }
}

public class BoardView : MonoBehaviour, IBoardView
{
    [SerializeField] private Deck[] decks;
    [SerializeField] private SeaView sea;
    [SerializeField] private ShipPlayerView shipPlayer;
    [SerializeField] private ShipMerchantView shipMerchant;
    [SerializeField] private ShipPirateView shipPirate;

    private BoardMode lastMode;

    public IShipView[] Ships => new IShipView[] {shipPlayer, shipMerchant, shipPirate};
    public IDeck[] Decks => decks;
    public ISeaView Sea => sea;
    
    public Vector3 PirateDockingPosition { get; private set; }
    public Vector3 MerchantDockingPosition { get; private set; }
    public Vector3 PirateSailingDestination { get; private set; }
    public Vector3 MerchantSailingDestination { get; private set; }

    [Inject]
    private void Construct(IViewportProvider viewportProvider)
    {
        var viewportHeight = viewportProvider.GetViewport(0).Size.y;
        
        transform.position = Vector3.down * (viewportHeight * 0.5f);

        shipPlayer.transform.localPosition = Vector3.up * shipPlayer.Height * 0.5f;
        sea.transform.localPosition = Vector3.up * (shipPlayer.Height + sea.Height);

        PirateDockingPosition = Vector3.up * (shipPlayer.Height + shipPirate.Height * 0.5f + 1f);
        MerchantDockingPosition = Vector3.up * (shipPlayer.Height + shipMerchant.Height * 0.5f);
        PirateSailingDestination = Vector3.up * (viewportHeight + shipPirate.Height * 0.5f);
        MerchantSailingDestination = Vector3.up * (viewportHeight + shipMerchant.Height * 0.5f);
    }
}
