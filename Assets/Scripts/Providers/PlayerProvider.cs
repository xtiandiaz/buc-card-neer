using System;

public interface IPlayerProvider : ICardProvider
{}

public class PlayerProvider : IPlayerProvider
{
    private readonly ICardPlayer playerCard;
    private readonly CardFactory cardFactory;

    private PlayerProvider(ICardPlayer playerCard, CardFactory cardFactory)
    {
        this.playerCard = playerCard;
        this.cardFactory = cardFactory;
    }
    
    public IObservable<ICard> WhenProvided { get; }
    
    public ICard Provide()
    {
        return cardFactory.Create(playerCard);
    }
}