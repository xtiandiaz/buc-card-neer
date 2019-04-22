using System;
using UniRx;
using Zenject;

[Flags]
public enum CardType
{
    Player      = 1 << 0,
    Item        = 1 << 1, 
    Merchant    = 1 << 2,
    Pirate      = 1 << 3,
    Inspector   = 1 << 4
}

public interface ICard
{
    CardType Type { get; }
    CardType InteractionMask { get; }
}

public abstract class Card : ICard
{
    public class Factory : IFactory<CardType, ICard>
    {
        private readonly PlayerCard.Factory playerCardFactory;
        private readonly ItemCard.Factory itemCardFactory;
        private readonly MerchantCard.Factory merchantCardFactory;
        private readonly PirateCard.Factory pirateCardFactory;

        private Factory(
            PlayerCard.Factory playerCardFactory,
            ItemCard.Factory itemCardFactory,
            MerchantCard.Factory merchantCardFactory, 
            PirateCard.Factory pirateCardFactory
            )
        {
            this.playerCardFactory = playerCardFactory;
            this.itemCardFactory = itemCardFactory;
            this.merchantCardFactory = merchantCardFactory;
            this.pirateCardFactory = pirateCardFactory;
        }

        public ICard Create(CardType forType)
        {
            switch (forType)
            {
                case CardType.Player:
                    return playerCardFactory.Create();
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
    
    public abstract CardType InteractionMask { get; }
    public CardType Type { get; }
}