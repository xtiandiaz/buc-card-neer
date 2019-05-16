using System;
using UniRx;
using Zenject;

public interface ISlotFactory : IFactory<ISlotView, ISlot>, IDisposable
{
}

public class SlotFactory : ISlotFactory
{
    private readonly SlotBoarding.Factory modelBoardingFactory;
    private readonly SlotPlayer.Factory modelPlayerFactory;
    private readonly SlotStorage.Factory modelResourceFactory;
    private readonly SlotEvent.Factory modelEventFactory;
    private readonly SlotController.Factory controllerFactory;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private SlotFactory(
        SlotBoarding.Factory modelBoardingFactory,
        SlotPlayer.Factory modelPlayerFactory,
        SlotStorage.Factory modelResourceFactory,
        SlotEvent.Factory modelEventFactory,
        SlotController.Factory controllerFactory
        )
    {
        this.modelBoardingFactory = modelBoardingFactory;
        this.modelPlayerFactory = modelPlayerFactory;
        this.modelResourceFactory = modelResourceFactory;
        this.modelEventFactory = modelEventFactory;
        this.controllerFactory = controllerFactory;
    }
    
    public ISlot Create(ISlotView fromView)
    {
        var model = CreateModel(fromView);
       
        disposables.Add(controllerFactory.Create(model, fromView));

        return model;
    }

    private ISlot CreateModel(ISlotView fromView)
    {
        var settings = fromView.Settings;
        var pileExtent = settings.Capacity > 0 ? (int) settings.Capacity : default(int?);
        var pile = new Pile(settings.Arrangement, pileExtent);
        
        switch (settings.Type)
        {
            case SlotType.Event:
                return modelEventFactory.Create(pile, settings, fromView.Bounds, fromView.Transform);
            case SlotType.Boarding:
                return modelBoardingFactory.Create(pile, settings, fromView.Bounds, fromView.Transform);
            case SlotType.Storage:
                return modelResourceFactory.Create(pile, settings, fromView.Bounds, fromView.Transform);
            case SlotType.Player:
                return modelPlayerFactory.Create(pile, settings, fromView.Bounds, fromView.Transform);
            default:
                throw new ArgumentOutOfRangeException(nameof(settings.Type), settings.Type, null);
        }
    }

    public void Dispose()
    {
        disposables?.Dispose();
    }
}