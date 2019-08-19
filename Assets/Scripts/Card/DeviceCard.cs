using Zenject;

public interface IDeviceCard : ICard
{
    DeviceType DeviceType { get; }
    bool IsLodgeable { get; }
}

public class DeviceCard : Card, IDeviceCard
{
    private DeviceCard(IDeviceCardModel model, IDeviceCardView view) 
        : base(model, view)
    {
        DeviceType = model.DeviceType;
        
        view.FaceColor = model.FaceColor;
        view.Name = model.Name;
    }
    
    public DeviceType DeviceType { get; }
    public bool IsLodgeable => (DeviceType & (DeviceType.Catapult)) != 0;

    public new class Factory : PlaceholderFactory<IDeviceCardModel, IDeviceCardView, DeviceCard>
    {
    }
}