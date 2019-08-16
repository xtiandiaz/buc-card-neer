using UnityEngine;

public interface IDeviceCardModel : ICardModel
{
    DeviceType DeviceType { get; }
    Color FaceColor { get; }
}

[CreateAssetMenu(menuName = "Model/Card/Device")]
public class DeviceCardModel : CardModel, IDeviceCardModel
{
    [SerializeField] private DeviceType deviceType = default;
    [SerializeField] private Color faceColor = default;

    public override CardType Type => CardType.Device;
    public DeviceType DeviceType => deviceType;
    public Color FaceColor => faceColor;
}