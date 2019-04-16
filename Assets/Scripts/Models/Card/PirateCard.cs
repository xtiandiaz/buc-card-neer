using UniRx;
using Zenject;

public class PirateCard : Card
{
    public class Factory : PlaceholderFactory<PirateCard>
    {
    }
    
    private readonly ReactiveProperty<int> stamina;
    private readonly ReactiveProperty<int> attack;
    
    protected PirateCard() : base(CardType.Pirate)
    {
    }
}