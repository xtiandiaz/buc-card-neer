using UnityEngine;
using Zenject;

public interface IBoardView
{
    ISkyView Sky { get; }
    ISeaView Sea { get; }
    IShipView Ship { get; }
    Vector3 WorldPosition { get; set; }
}

public class BoardView : MonoBehaviour, IBoardView
{
    [SerializeField] private SkyView sky = default;
    [SerializeField] private SeaView sea = default;
    [SerializeField] private ShipView ship = default;

    public ISkyView Sky => sky;
    public ISeaView Sea => sea;
    public IShipView Ship => ship;
    
    public Vector3 WorldPosition
    {
        get => transform.position;
        set => transform.position = value;
    }

    [Inject]
    private void Initialize(
        IViewportProvider viewportProvider,
        IBoardModel model
        )
    {
        var seaHeight = sea.Height;
        var seaDepth = sea.ZDepth;
        
        var nearViewport = viewportProvider.GetViewport(0);
        var farViewport = viewportProvider.GetViewport(seaDepth);

        var nearViewportCenter = nearViewport.Size * 0.5f;
        
        ship.LocalPosition = Vector3.up * model.Padding.y;
        sea.LocalPosition = ship.LocalPosition + Vector3.up * (ship.HullHeight + ship.HullTopMargin);

        var skyScale = sky.LocalScale;
        skyScale.y = farViewport.Size.y - sea.LocalPosition.y - seaHeight;
        sky.LocalScale = skyScale;
        
        sky.LocalPosition = sea.LocalPosition + Vector3.up * seaHeight + Vector3.forward * sea.ZDepth;
        
        WorldPosition = Vector3.down * (nearViewportCenter.y);
    }
}
