using Zenject;
using Random = UnityEngine.Random;

public interface IItemCard
{
    CardType Type { get; }
    int Value { get; }
}

public class ItemCard : Card, IItemCard
{
    public class Factory : PlaceholderFactory<CardType, ItemCard>
    {
    }
    
    protected ItemCard(CardType cardType) : base(cardType)
    {
        switch (cardType)
        {
            case CardType.Health:
                Value = Random.Range(1, 4);
                break;
            case CardType.Stamina:
                Value = Random.Range(3, 11);
                break;
            case CardType.Defense:
                Value = Random.Range(3, 6);
                break;
        }
    }
    
    public int Value { get; }
}