using Zenject;
using Random = UnityEngine.Random;

public class ItemCard : Card
{
    public class Factory : PlaceholderFactory<ItemCard>
    {
    }
    
    protected ItemCard() : base(CardType.Item)
    {
    }
}