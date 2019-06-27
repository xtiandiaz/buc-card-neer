using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IDeckModel
{
    bool ShouldShuffleOnInit { get; }
    
    IEnumerable<ICardModel> Pirates { get; }
    IEnumerable<ICardModel> Merchants { get; }
    IEnumerable<ICardModel> Inspectors { get; }
    IEnumerable<ICardModel> Items { get; }
    IEnumerable<ICardModel> Tools { get; }
    IEnumerable<ICardModel> Monsters { get; }
}

[CreateAssetMenu(menuName = "Model/Deck")]
public class DeckModel : ScriptableObject, IDeckModel
{
    [SerializeField] private bool shouldShuffleOnInit = true;
    [SerializeField] private CardModel[] pirates;
    [SerializeField] private CardModel[] merchants;
    [SerializeField] private CardModel[] inspectors;
    [SerializeField] private CardModel[] items;
    [SerializeField] private CardModel[] tools;
    [SerializeField] private CardModel[] monsters;

    public bool ShouldShuffleOnInit => shouldShuffleOnInit;
    
    public IEnumerable<ICardModel> Pirates => pirates.Select(Instantiate);
    public IEnumerable<ICardModel> Merchants => merchants.Select(Instantiate);
    public IEnumerable<ICardModel> Inspectors => inspectors.Select(Instantiate);
    public IEnumerable<ICardModel> Items => items.Select(Instantiate);
    public IEnumerable<ICardModel> Tools => tools.Select(Instantiate);
    public IEnumerable<ICardModel> Monsters => monsters.Select(Instantiate);
}