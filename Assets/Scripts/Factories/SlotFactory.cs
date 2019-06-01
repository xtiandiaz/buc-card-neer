using System;
using UniRx;
using Zenject;

public interface ISlotFactory : IFactory<ISlotView, ISlot>, IDisposable
{
}

public class SlotFactory : ISlotFactory
{
    private readonly BoardingSlot.Factory modelBoardingFactory;
    private readonly PlayerSlot.Factory modelPlayerFactory;
    private readonly StorageSlot.Factory modelResourceFactory;
    private readonly SupplySlot.Factory modelEventFactory;
    private readonly SlotController.Factory controllerFactory;
    private readonly BoardingSlotController.Factory controllerFactoryBoarding;
    private readonly StorageSlotController.Factory controllerFactoryStorage;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private SlotFactory(
        BoardingSlot.Factory modelBoardingFactory,
        PlayerSlot.Factory modelPlayerFactory,
        StorageSlot.Factory modelResourceFactory,
        SupplySlot.Factory modelEventFactory,
        SlotController.Factory controllerFactory,
        BoardingSlotController.Factory controllerFactoryBoarding,
        StorageSlotController.Factory controllerFactoryStorage
        )
    {
        this.modelBoardingFactory = modelBoardingFactory;
        this.modelPlayerFactory = modelPlayerFactory;
        this.modelResourceFactory = modelResourceFactory;
        this.modelEventFactory = modelEventFactory;
        this.controllerFactory = controllerFactory;
        this.controllerFactoryBoarding = controllerFactoryBoarding;
        this.controllerFactoryStorage = controllerFactoryStorage;
    }
    
    public ISlot Create(ISlotView fromView)
    {
        var model = CreateModel(fromView);
       
        disposables.Add(CreateController(model, fromView));

        return model;
    }

    private ISlot CreateModel(ISlotView fromView)
    {
        var settings = fromView.Settings;
        var pileExtent = settings.Capacity > 0 ? (int) settings.Capacity : default(int?);
        var pile = new Pile(settings.Arrangement, pileExtent);
        
        switch (settings.Type)
        {
            case SlotType.Supply:
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

    private ISlotController CreateController(ISlot forModel, ISlotView andView)
    {
        switch (forModel.Type)
        {
            case SlotType.Boarding:
                return controllerFactoryBoarding.Create((IBoardingSlot) forModel, andView);
            case SlotType.Storage:
                return controllerFactoryStorage.Create((IStorageSlot) forModel, (IStorageSlotView) andView);
            default:
                return controllerFactory.Create(forModel, andView);
        }
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}