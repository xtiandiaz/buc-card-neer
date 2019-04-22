using Zenject;

public class PlayerCard : Card
{
    public class Factory : PlaceholderFactory<PlayerCard>
    {
    }
    
    private readonly GameSettings gameSettings;
    
    protected PlayerCard(GameSettings gameSettings) : base(CardType.Player)
    {
        this.gameSettings = gameSettings;
    }
    
    public override CardType InteractionMask { get; } = CardType.Player;
}