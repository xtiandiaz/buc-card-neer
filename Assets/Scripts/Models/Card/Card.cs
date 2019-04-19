using System;
using UniRx;
using Zenject;

public enum CardType
{
    Player,
    Item, 
    Merchant,
    Pirate,
    Inspector
}

public interface ICard
{
    CardType Type { get; }

    void Enter(ICardSlot slot);
    void Leave(ICardSlot slot);
}

public abstract class Card : ICard
{
    public class Factory : IFactory<CardType, ICard>
    {
        private readonly ItemCard.Factory itemCardFactory;
        private readonly MerchantCard.Factory merchantCardFactory;
        private readonly PirateCard.Factory pirateCardFactory;

        private Factory(
            ItemCard.Factory itemCardFactory,
            MerchantCard.Factory merchantCardFactory, 
            PirateCard.Factory pirateCardFactory
            )
        {
            this.itemCardFactory = itemCardFactory;
            this.merchantCardFactory = merchantCardFactory;
            this.pirateCardFactory = pirateCardFactory;
        }

        public ICard Create(CardType forType)
        {
            switch (forType)
            {
                case CardType.Item:
                    return itemCardFactory.Create();
                case CardType.Merchant:
                    return merchantCardFactory.Create();
                case CardType.Pirate:
                    return pirateCardFactory.Create();
                default:
                    throw new ArgumentOutOfRangeException(nameof(forType), forType, null);
            }
        }
    }
    
    protected Card(CardType type)
    {
        Type = type;
    }
    
    public CardType Type { get; }
    
    public void Enter(ICardSlot slot)
    {
        throw new NotImplementedException();
    }

    public void Leave(ICardSlot slot)
    {
        throw new NotImplementedException();
    }
}