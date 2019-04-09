using Zenject;
using Random = UnityEngine.Random;

public interface IResourceCard
{
    CardType Type { get; }
    int Value { get; }
}

public class ResourceCard : Card, IResourceCard
{
    public class Factory : PlaceholderFactory<CardType, ResourceCard>
    {
    }
    
    protected ResourceCard(CardType cardType) : base(cardType)
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