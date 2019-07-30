using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

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
        
        // TODO Optimize and move to a separate class Shuffling Strategy
        
        var suits = new List<CardType> {CardType.Food, CardType.Artifact, CardType.Gem};
        suits.Shuffle();

        var suitOrder = Enumerable.Range(0, suits.Count)
            .ToDictionary(key => suits[key], value => value);

        var itemsPerSuit = fromModel.Items.Count(item => item.Suit.Type == CardType.Food);

        var items = fromModel.Items
            .OrderBy(item => suitOrder[item.Suit.Type])
            .Select((item, i) => new {Index = i % itemsPerSuit, Item = item})
            .OrderBy(itemObj => itemObj.Index)
            .Select(itemObj => itemObj.Item)
            .ToList();

        cardData.AddRange(items);
        
        var merchantsPerSuit = fromModel.Merchants.Count(merchant => merchant.Suit.Type == CardType.Food);
        var merchants = fromModel.Merchants
            .OrderBy(merchant => suitOrder[merchant.Suit.Type])
            .Select((merchant, i) => new {Index = i % merchantsPerSuit, Merchant = merchant})
            .OrderBy(merchantObj => merchantObj.Index)
            .Select(merchantObj => merchantObj.Merchant)
            .ToList();

        cardData = cardData.Apportion(merchants);

        var monsterPerSuit = fromModel.Monsters.Count(monster => monster.Suit.Type == CardType.Food);
        var monsters = fromModel.Monsters
            .OrderBy(monster => suitOrder[monster.Suit.Type])
            .Select((monster, i) => new {Index = i % monsterPerSuit, Monster = monster})
            .OrderBy(monsterObj => monsterObj.Index)
            .Select(monsterObj => monsterObj.Monster)
            .ToList();

        cardData = cardData.Apportion(monsters);
        
        var pirates = fromModel.Pirates.ToList();
        pirates.Shuffle();
        
        cardData = cardData.Apportion(pirates);
        
        var tools = fromModel.Tools.ToList();
        tools.Shuffle();
        
        cardData = cardData.Apportion(tools);
        
        /*var inspectors = fromModel.Inspectors.ToList();
        inspectors.Shuffle();
        
        cardData = cardData.Apportion(inspectors);*/

        var deck = deckFactory.Create(cardData);
        
        disposables.Add(deck);
        
        return deck;
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

    public static List<T> Apportion<T>(this IList<T> population, IList<T> sample) where T : class
    {
        var totalCount = population.Count + sample.Count;
        var totalCountM1 = totalCount - 1;
        var trail = 0f;
        var result = new T[totalCount];

        for (var i = 0; i < sample.Count; i++)
        {
            var step = (totalCount - trail) / (sample.Count - i);
            //Debug.Log($"Step: {step}");
            
            trail += Random.Range(1f, step);
            //Debug.Log($"Trail: {trail}");

            var ai = Mathf.Clamp(Mathf.RoundToInt(trail), 0, totalCountM1);
            //Debug.Log(ai);

            result[ai] = sample[i];
        }

        var j = 0;
        for (var i = 0; i < totalCount; i++)
        {
            if (result[i] != null)
                continue;

            result[i] = population[j];
            j++;
        }

        return result.ToList();
    }
}