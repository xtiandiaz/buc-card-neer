using System;
using UniRx;
using Zenject;
using UnityEngine;

public interface ICardFactory : IFactory<ICardModel, ICard>, IDisposable
{
}

public class CardFactory : ICardFactory
{
    private readonly DiContainer container;
    private readonly Card.Factory cardFactory;
    private readonly PlayerCard.Factory playerFactory;
    private readonly MerchantCard.Factory merchantFactory;
    private readonly DeviceCard.Factory deviceFactory;
    private readonly Viewport viewport;
    private readonly ILocalizationProvider localizator;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private CardFactory(
        DiContainer container,
        Card.Factory cardFactory,
        PlayerCard.Factory playerFactory,
        MerchantCard.Factory merchantFactory,
        DeviceCard.Factory deviceFactory,
        Viewport viewport,
        ILocalizationProvider localizator
    )
    {
        this.container = container;
        this.cardFactory = cardFactory;
        this.playerFactory = playerFactory;
        this.merchantFactory = merchantFactory;
        this.deviceFactory = deviceFactory;
        this.viewport = viewport;
        this.localizator = localizator;
    }

    public ICard Create(ICardModel fromModel)
    {
        PrepareModel(fromModel);
        
        var view = CreateView(fromModel);
        var card = CreateCard(fromModel, view);

        disposables.Add(card);

        return card;
    }

    private ICard CreateCard(ICardModel withModel, ICardView view)
    {
        switch (withModel.Type)
        {
            case CardType.Player:
                return playerFactory.Create((IPlayerCardModel) withModel, (IPlayerCardView) view);
            case CardType.Merchant:
                return merchantFactory.Create(withModel, (IMerchantCardView) view);
            case CardType.Device:
                return deviceFactory.Create((IDeviceCardModel) withModel, (IDeviceCardView) view);
            default:
                return cardFactory.Create(withModel, view);
        }
    }

    private ICardView CreateView(ICardModel fromModel)
    {
        var view = container.InstantiatePrefabForComponent<ICardView>(fromModel.ViewPrefab);

        view.FrontCover = fromModel.FrontCover;
        view.BackCover = fromModel.BackCover;
        view.FrontMotif = fromModel.FrontMotif;
        view.BackMotif = fromModel.BackMotif;
        
        view.ToggleValueVisibility(fromModel.ShouldDisplayValue);

        view.Position = (viewport.Size.y + GameStatics.CardHeight) * 0.5f *
                        ((fromModel.Type & CardType.Player) != 0 ? Vector3.down : Vector3.up) +
                        5f * Vector3.back;

        return view;
    }

    private void PrepareModel(ICardModel model)
    {
        if (model is IDeviceCardModel deviceModel)
        {
            model.Name = localizator.GetName(deviceModel.DeviceType);
        }
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}