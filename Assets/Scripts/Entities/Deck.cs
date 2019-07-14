using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Zenject;

public interface IDeck : IDisposable
{
    bool IsExhausted { get; }
    
    IObservable<int> CardCount { get; }

    void Shuffle();
    ICard Provide();
    ICard Provide(CardType ofType);
    bool DoesContain(CardType type);
}

public class Deck : IDeck
{
    public class Factory : PlaceholderFactory<List<ICardModel>, Deck>
    {
    }

    private readonly ICardFactory cardFactory;
    private readonly List<ICardModel> cardModels;
    private readonly Subject<ICard> provision = new Subject<ICard>();
    private readonly ReactiveProperty<int> cardCount;

    private Deck(List<ICardModel> cardModels, ICardFactory cardFactory)
    {
        this.cardModels = cardModels;
        this.cardFactory = cardFactory;
        
        cardCount = new ReactiveProperty<int>(cardModels.Count);
    }
    
    public bool IsExhausted { get; private set; }
    public IObservable<int> CardCount => cardCount;
    
    public void Shuffle()
    {
        cardModels.Shuffle();
    }

    public ICard Provide()
    {
        return Provide(cardModels.LastOrDefault());
    }

    public ICard Provide(CardType ofType)
    {
        return Provide(cardModels.FirstOrDefault(card => card.Type == ofType));
    }

    public bool DoesContain(CardType type)
    {
        return cardModels.FirstOrDefault(card => card.Type == type) != null;
    }

    private ICard Provide(ICardModel withModel)
    {
        if (withModel == null)
            return null;
        
        var card = cardFactory.Create(withModel);
        
        cardModels.Remove(withModel);
        provision.OnNext(card);
        
        cardCount.Value = cardModels.Count;

        IsExhausted = cardCount.Value <= 0;

        return card;
    }

    public void Dispose()
    {
        provision?.Dispose();
    }
}