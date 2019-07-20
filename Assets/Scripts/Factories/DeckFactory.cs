using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Zenject;

public interface IDeckFactory : IFactory<IDeckModel, IDeck>, IDisposable
{
}

public class DeckFactory : IDeckFactory
{
    private readonly Deck.Factory deckFactory;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private DeckFactory(
        Deck.Factory deckFactory
        )
    {
        this.deckFactory = deckFactory;
    }
    
    public IDeck Create(IDeckModel fromModel)
    {
        var cardData = new List<ICardModel>();

        /* var tools = fromModel.Tools.ToList();
        tools.Shuffle();
        
        //cardData.AddRange(tools);
        
       var items = fromModel.Items
            .OrderBy(item => item.Suit.Type)
            .Select((item, i) => new {Index = i % 4, Item = item})
            .OrderBy(itemObj => itemObj.Index)
            .Select(itemObj => itemObj.Item)
            .ToList();

        cardData.AddRange(items);*/

        cardData.AddRange(fromModel.Pirates);
        cardData.AddRange(fromModel.Merchants);
        cardData.AddRange(fromModel.Inspectors);
        cardData.AddRange(fromModel.Items);
        cardData.AddRange(fromModel.Tools);
        cardData.AddRange(fromModel.Monsters);
        
        if (fromModel.ShouldShuffleOnInit)
            cardData.Shuffle();
        
        var controller = deckFactory.Create(cardData);
        
        disposables.Add(controller);
        
        return controller;
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}

public static class ShufflingUtils
{
    private static readonly System.Random Rng = new System.Random();
    
    public static void Shuffle<T>(this IList<T> list)  
    {  
        var n = list.Count;
        
        while (n > 1) 
        {  
            n--;  
            
            var k = Rng.Next(n + 1);  
            var value = list[k];  
            
            list[k] = list[n];  
            list[n] = value;  
        } 
    }

    public static void Apportion<T>(this IList<T> intoList, IList<T> sample)
    {
        var si = sample.Count - 1;
        var ai = 0;
        
        while (si > 0)
        {
            var item = sample[si];

            //intoList.Insert(0, );
            
            si--;
        }

    }
}