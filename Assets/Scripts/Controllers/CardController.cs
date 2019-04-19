using System;
using UniRx;
using UnityEngine;
using Zenject;

public enum CardInteractionEventType
{
    Pick,
    Drop
}

public struct CardInteractionEvent
{
    public CardInteractionEventType type;
    public ICardController card;

    public CardInteractionEvent(
        CardInteractionEventType type,
        ICardController card
    )
    {
        this.type = type;
        this.card = card;
    }
}

public interface ICardController
{
    Transform Transform { get; }
    IConnectableObservable<CardInteractionEvent> InteractionEvent { get; }

    void Arrange(Vector3 atLocalPos, int andIndexInSlot);
}

public class CardController : ICardController
{
    public class Factory : PlaceholderFactory<ICard, ICardView, CardController>
    {
        private readonly ItemCardView.Factory resourceCardViewFactory;
        private readonly PirateCardView.Factory pirateCardViewFactory;
        private readonly MerchantCardView.Factory merchantCardViewFactory;

        private Factory(
            ItemCardView.Factory resourceCardViewFactory,
            PirateCardView.Factory pirateCardViewFactory,
            MerchantCardView.Factory merchantCardViewFactory
            )
        {
            this.resourceCardViewFactory = resourceCardViewFactory;
            this.pirateCardViewFactory = pirateCardViewFactory;
            this.merchantCardViewFactory = merchantCardViewFactory;
        }

        public CardController Create(ICard model)
        {
            return base.Create(model, CreateView(model));
        }

        private CardView CreateView(ICard fromModel)
        {
            switch (fromModel.Type)
            {
                case CardType.Item:
                    return resourceCardViewFactory.Create(GetResourceName(fromModel.Type));
                case CardType.Merchant:
                    return merchantCardViewFactory.Create(GetResourceName(fromModel.Type));
                case CardType.Pirate:
                    return pirateCardViewFactory.Create(GetResourceName(fromModel.Type));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private string GetResourceName(CardType cardType)
        {
            return $"Prefabs/Cards/{cardType.ToString()}";
        }
    }

    private readonly ICard model;
    private readonly ICardView view;
    private readonly GameSettings settings;
    private readonly IDisposable interactionEventConnection;
    
    private CardController(
        ICard model, 
        ICardView view, 
        GameSettings settings
        )
    {
        this.model = model;
        this.view = view;
        this.settings = settings;

        InteractionEvent = view.InteractionEvent
                .Select(eventType => new CardInteractionEvent(eventType, this))
                .Publish();

        interactionEventConnection = InteractionEvent.Connect();
    }

    public Transform Transform => view.Transform;

    public IConnectableObservable<CardInteractionEvent> InteractionEvent { get; }

    public void Arrange(Vector3 atLocalPos, int andIndexInSlot)
    {
        view.Arrange(atLocalPos, andIndexInSlot);
    }

    public void Dispose()
    {
        interactionEventConnection.Dispose();
    }
}