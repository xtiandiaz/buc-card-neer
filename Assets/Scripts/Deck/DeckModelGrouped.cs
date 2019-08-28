using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IDeckModelGrouped : IDeckModel
{
    bool ShouldShuffleOnInit { get; }
    
    IEnumerable<ICardModel> Pirates { get; }
    IEnumerable<ICardModel> Merchants { get; }
    IEnumerable<ICardModel> Monsters { get; }
    IEnumerable<ICardModel> Items { get; }
    IEnumerable<ICardModel> Tools { get; }
}

[CreateAssetMenu(menuName = "Model/Deck/Grouped")]
public class DeckModelGrouped : DeckModel, IDeckModelGrouped
{
    [SerializeField] private bool shouldShuffleOnInit = true;
    [SerializeField] private CardModel[] pirates = default;
    [SerializeField] private CardModel[] merchants = default;
    [SerializeField] private CardModel[] monsters = default;
    [SerializeField] private CardModel[] items = default;
    [SerializeField] private CardModel[] tools = default;

    public bool ShouldShuffleOnInit => shouldShuffleOnInit;
    
    public IEnumerable<ICardModel> Pirates => pirates.Select(Instantiate);
    public IEnumerable<ICardModel> Merchants => merchants.Select(Instantiate);
    public IEnumerable<ICardModel> Monsters => monsters.Select(Instantiate);
    public IEnumerable<ICardModel> Items => items.Select(Instantiate);
    public IEnumerable<ICardModel> Tools => tools.Select(Instantiate);
}