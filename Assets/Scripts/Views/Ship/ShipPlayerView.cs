using Zenject;

public interface IShipPlayerView : IShipView
{
}

public class ShipPlayerView : ShipView, IShipPlayerView
{
    public class Factory : PlaceholderFactory<IShipPlayerView>
    {
    }
}