using System;
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
    
    private static readonly TimeSpan ShootingDelay = TimeSpan.FromSeconds(0.25);

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
        #region Boarding & Storing

        disposables.Add(model.WhenBoarded
            .Do(card =>
            {
                if (!(card is IResourceCard resourceCard) || !resourceCard.IsLocked)
                    card.Flip(CardFace.Front);
            })
            .Where(card => card is IResourceCard)
            .Cast<ICard, IResourceCard>()
            .Delay(TimeSpan.FromSeconds(cardAnimationSettings.FlipDuration))
            .SelectMany(resCard =>
                resCard.IsLocked ? resCard.WhenUnlocked.Select(_ => resCard) : Observable.Return(resCard))
            .Do(model.Store)
            .Subscribe());

        #endregion

        #region Shooting

        disposables.Add(model.WhenArmed
            .Do(_ => model.Lock())
            .Delay(ShootingDelay)
            .Do(_ => model.Shoot())
            .Subscribe());

        #endregion
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}