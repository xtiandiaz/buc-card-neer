using System;
using UniRx;
using UnityEngine;
using Zenject;

public interface IBoardController : IInitializable, IDisposable
{
}

public class BoardController : IBoardController
{
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    
    private readonly ISea sea;
    private readonly IShip ship;
    private readonly IPlayerCard player;
    private readonly IShootingController shooter;
    private readonly IDealingController dealer;
    private readonly IFloatingBannerFactory bannerFactory;

    private BoardController(
        ISea sea,
        IShip ship,
        IPlayerCard player,
        IShootingController shooter,
        IDealingController dealer,
        IFloatingBannerFactory bannerFactory
    )
    {
        this.sea = sea;
        this.ship = ship;
        this.player = player;
        this.shooter = shooter;
        this.dealer = dealer;
        this.bannerFactory = bannerFactory;
    }

    public void Initialize()
    {
        disposables.Add(ship.WhenCardBoarded.Take(1)
            .Do(_ => sea.Lock())
            .ContinueWith(ship.WhenCardHandled.Take(1))
            .ContinueWith(sea.Clash())
            .ContinueWith(_ => sea.Arrange())
            .ContinueWith(sea.Resupply())
            .DoOnCompleted(sea.Unlock)
            .RepeatSafe()
            .Subscribe());
        
        disposables.Add(ship.WhenArmed.Take(1)
            .Do(_ =>
            {
                sea.Lock();
                ship.Lock();
            })
            .Delay(TimeSpan.FromSeconds(0.5))
            .ContinueWith(_ => shooter.Shoot(ship.Plank, sea.Slots))
            .ContinueWith(_ => sea.Arrange())
            .ContinueWith(sea.Resupply())
            .DoOnCompleted(() =>
            {
                sea.Unlock();
                ship.Unlock();
            })
            .RepeatSafe()
            .Subscribe());
        
        disposables.Add(player.WhenHealed
            .Merge(player.WhenHitOrHacked.Select(value => -value))
            .Subscribe(byAmount => 
                bannerFactory.Create(
                        FloatingBannerType.Health, 
                        byAmount > 0 ? $"+{byAmount}" : $"{byAmount}", 
                        player.Position)
                    .Show(
                        byAmount > 0 ? FloatingBanner.DisplayMode.FadeInUpward : FloatingBanner.DisplayMode.FadeInDownward, 
                        1f, 
                        true)));

        /*disposables.Add(player.WhenCredited
              .Subscribe(byAmount => 
                  bannerFactory.Create(FloatingBannerType.Coins, $"+{byAmount}", player.Position - Vector3.up * 0.25f)
                      .Show(FloatingBanner.DisplayMode.FadeInDownward, 2f, true)));*/
        
        disposables.Add(dealer.WhenDealt
            .Where(card => card.IsAgent || card.IsMonster)
            .SelectMany(card => card.WhenHitOrHacked
                .TakeUntil(card.WhenDestroyed)
                .Do(byAmount => bannerFactory.Create(FloatingBannerType.Health, $"-{byAmount}", card.Position)
                    .Show(FloatingBanner.DisplayMode.FadeInDownward, 1f, true)))
            .Subscribe());
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}