using System.Collections.Generic;
using UnityEngine;

public interface IDeckModelMixed : IDeckModel
{
    IEnumerable<ICardModel> CardModels { get; }
}

[CreateAssetMenu(menuName = "Model/Deck/Mixed")]
public class DeckModelMixed : DeckModel, IDeckModelMixed
{
    [SerializeField] private CardModel[] cardModels = default;

    public IEnumerable<ICardModel> CardModels => cardModels;
}