using UnityEngine;

public interface IDeviceCardView : ICardView
{
    Color FaceColor { set; }
}

public class DeviceCardView : CardView, IDeviceCardView
{
    public Color FaceColor
    {
        set => customizer.FrontCoverColor = value;
    }
}