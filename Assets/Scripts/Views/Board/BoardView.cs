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
    
    [SerializeField] private SeaView sea;
    [SerializeField] private ShipView shipPlayer;

    [Inject]
    private void Initialize(Viewport viewport)
    {
        var viewportHeight = viewport.Size.y;
        
        transform.position = Vector3.down * (viewportHeight * 0.5f);

        shipPlayer.transform.localPosition = Vector3.up * shipPlayer.Height * 0.5f;
        sea.transform.localPosition = Vector3.up * (shipPlayer.Height + sea.Height);
    }
}
