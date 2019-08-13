using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Zenject;

public interface IDeck : IDisposable
{
    bool IsExhausted { get; }
    
    IObservable<int> CardCount { get; }
    
    ICard Provide();
    IList<ICard> Provide(int count);
    ICard Provide(CardType ofType);
    bool DoesContain(CardType type);
}

public class Deck : IDeck
{
    public class Factory : PlaceholderFactory<List<ICardModel>, Deck>
    {
    }

    private readonly Subject<ICard> provision = new Subject<ICard>();
    private readonly ReactiveProperty<int> cardCount;
    
    private readonly ICardFactory cardFactory;
    private readonly IBoardModel boardModel;
    private readonly Stack<ICardModel> modelStack;

    private Deck(
        List<ICardModel> cardModels, 
        ICardFactory cardFactory,
        IBoardModel boardModel
        )
    {
        this.cardFactory = cardFactory;
        this.boardModel = boardModel;

        modelStack = new Stack<ICardModel>(cardModels);
        
        cardCount = new ReactiveProperty<int>(cardModels.Count);
    }
    
    public bool IsExhausted { get; private set; }
    public IObservable<int> CardCount => cardCount;

    public IList<ICard> Provide(int count)
    {
        var cards = new List<ICard>();

        for (var i = 0; i < count; i++)
        {
            var card = Provide();
            if (card == null)
                break;
            
            cards.Add(card);
        }

        if (cardCount.Value <= boardModel.MaxCardsInSupply) 
            cards = cards
                .OrderByDescending(c => c.IsResource)
                .ThenByDescending(c => !c.IsLocked)
                .ThenByDescending(c => !c.IsMerchant)
                .ToList();

        return cards;
    }
    
    public ICard Provide()
    {
        IsExhausted = modelStack.Count == 0;

        return IsExhausted ? null : Produce(modelStack.Pop());
    }

    public ICard Provide(CardType ofType)
    {
        return Produce(modelStack.FirstOrDefault(card => card.Type == ofType));
    }

    public bool DoesContain(CardType type)
    {
        return modelStack.FirstOrDefault(card => card.Type == type) != null;
    }

    private ICard Produce(ICardModel withModel)
    {
        if (withModel == null)
            return null;
        
        var card = cardFactory.Create(withModel);
        
        provision.OnNext(card);
        
        cardCount.Value = modelStack.Count;

        IsExhausted = cardCount.Value <= 0;

        return card;
    }

    public void Dispose()
    {
        provision?.Dispose();
    }
}