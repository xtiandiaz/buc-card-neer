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
    private readonly Viewport viewport;
    private readonly IBoardModel boardModel;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private CardFactory(
        DiContainer container,
        Card.Factory cardFactory,
        PlayerCard.Factory playerFactory, 
        IBoardModel boardModel,
        Viewport viewport
    )
    {
        this.container = container;
        this.cardFactory = cardFactory;
        this.playerFactory = playerFactory;
        this.viewport = viewport;
        this.boardModel = boardModel;
    }

    public ICard Create(ICardModel fromModel)
    {
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
            default:
                return cardFactory.Create(withModel, view);
        }
    }

    private ICardView CreateView(ICardModel fromModel)
    {
        var view = container.InstantiatePrefabForComponent<ICardView>(fromModel.ViewPrefab);

        view.Value = fromModel.Value;
        
        view.FrontCover = fromModel.FrontCover;
        view.BackCover = fromModel.BackCover;
        view.FrontMotif = fromModel.FrontMotif;
        view.BackMotif = fromModel.BackMotif;
        
        view.ToggleValueVisibility(fromModel.ShouldDisplayValue);

        view.Position = (viewport.Size.y + boardModel.CardSize.y) * 0.5f *
                        ((fromModel.Type & CardType.Player) != 0 ? Vector3.down : Vector3.up) +
                        5f * Vector3.back;

        return view;
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}