using Zenject;

public class DeviceCard : Card
{
    private DeviceCard(IDeviceCardModel model, IDeviceCardView view) 
        : base(model, view)
    {
        view.FaceColor = model.FaceColor;
        view.Name = model.Name;
    }

    public new class Factory : PlaceholderFactory<IDeviceCardModel, IDeviceCardView, DeviceCard>
    {
    }
}