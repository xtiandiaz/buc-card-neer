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
    private static readonly TimeSpan SelfStoringDelay = TimeSpan.FromSeconds(0.5);
    
    private readonly IShip model;
    private readonly IShipView view;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    protected ShipController(IShip model, IShipView view)
    {
        this.model = model;
        this.view = view;
    }

    [Inject]
    public void Initialize()
    {
        #region Boarding

        disposables.Add(model.WhenBoarded
            .Do(card =>
            {
                if (!(card is IResourceCard resourceCard) || !resourceCard.IsLocked)
                    card.Flip(CardFace.Front);
            })
            .Where(card => card is IResourceCard)
            .Cast<ICard, IResourceCard>()
            .Delay(SelfStoringDelay)
            .SelectMany(resCard =>
                resCard.IsLocked ? resCard.WhenUnlocked.Select(_ => resCard) : Observable.Return(resCard))
            .Do(resCard => model.Store(resCard))
            .Subscribe());

        #endregion

        #region Battling

        /*disposables.Add(model.WhenArmed
            .Do(_ => model.Lock())
            .Delay(ShootingDelay)
            .Do(_ => model.Shoot())
            .Subscribe());*/

        #endregion
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}