using System;
using System.Linq;
using UniRx;
using Zenject;

public interface IShipController : IInitializable, IDisposable
{
}

public class ShipController : IShipController
{
    public class Factory : PlaceholderFactory<IShip, IShipView, ShipController>
    {
    }

    private readonly IShip model;
    private readonly IShipView view;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    [Inject] private CardAnimationSettings cardAnimationSettings;

    protected ShipController(IShip model, IShipView view)
    {
        this.model = model;
        this.view = view;
    }

    [Inject]
    public void Initialize()
    {
        #region Arrangement

        disposables.Add(model.Slots
            .Select(slot => slot.WhenReleased.Select(_ => slot))
            .Merge()
            .Subscribe(slot => slot.Arrange()));

        #endregion
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}