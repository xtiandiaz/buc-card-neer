using System;
using UniRx;
using Zenject;

public interface ICardNotificationsController : IInitializable, IDisposable
{
}

public class CardNotificationsController : ICardNotificationsController
{
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    
    private readonly IFloatingBannerFactory floatingBannerFactory;
    private readonly IPlayerCard player;
    private readonly IDealingController dealer;

    private CardNotificationsController(
        IFloatingBannerFactory floatingBannerFactory,
        IPlayerCard player,
        IDealingController dealer
    )
    {
        this.floatingBannerFactory = floatingBannerFactory;
        this.player = player;
        this.dealer = dealer;
    }

    public void Initialize()
    {
        disposables.Add(player.WhenHealed
            .Merge(player.WhenHitOrHacked.Select(value => -value))
            .Subscribe(byAmount => 
                floatingBannerFactory.Create(
                        FloatingBannerType.Health, 
                        byAmount > 0 ? $"+{byAmount}" : $"{byAmount}", 
                        player.Position)
                    .Show(
                        byAmount > 0 ? FloatingBanner.DisplayMode.FadeInUpward : FloatingBanner.DisplayMode.FadeInDownward, 
                        1f, 
                        true)));

        disposables.Add(dealer.WhenDealt
            .Where(card => card.IsAgent || card.IsMonster)
            .SelectMany(card => card.WhenHitOrHacked
                .TakeUntil(card.WhenDestroyed)
                .Do(byAmount => floatingBannerFactory.Create(FloatingBannerType.Health, $"-{byAmount}", card.Position)
                    .Show(FloatingBanner.DisplayMode.FadeInDownward, 1f, true)))
            .Subscribe());
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}