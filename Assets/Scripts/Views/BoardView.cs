using UnityEngine;
using Zenject;

public interface IBoardView
{
}

public class BoardView : MonoBehaviour, IBoardView
{
    public class Factory : PlaceholderFactory<BoardView>
    {
    }
    
    [SerializeField] private SkyView sky;
    [SerializeField] private SeaView sea;
    [SerializeField] private ShipView ship;

    [Inject]
    private void Initialize(Viewport viewport)
    {
        var viewportCenter = viewport.Size * 0.5f;
        
        transform.position = Vector3.down * (viewportCenter.y);
        ship.LocalPosition = Vector3.zero;
        sea.LocalPosition = ship.LocalPosition + Vector3.up * ship.HullSize.y;
        sky.LocalPosition = sea.LocalPosition + Vector3.up * sea.Height;

        var skyScale = sky.LocalScale;
        sky.LocalScale = new Vector3(skyScale.x, viewport.Size.y - sky.LocalPosition.y, skyScale.z);
    }
}
